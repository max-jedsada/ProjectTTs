using Manager.Interfaces;
using Manager.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml.Packaging.Ionic.Zlib;
using Project.Manager.Exception;
using Project.Manager.Redis;
using Project.Manager.ResponseModels;
using Provider.Interfaces;
using Quartz;
using RestSharp;
using Serilog;
using System.Drawing;
using System.Net;
using System.Net.Mail;

namespace Manager.Services
{
    public class TestManager : ITestManager
    {
        private readonly ILogger<TestManager> _logger;

        public TimeSpan cacheDuration = TimeSpan.FromSeconds(900); // 900 = 15 min

        private readonly IConfiguration _configuration;
        private readonly ITestProvider _testProvider;
        private IRedisCacheService _redisCacheService;

        public TestManager(IConfiguration configuration,
                           ITestProvider testProvider,
                           IRedisCacheService redisCacheService,
                           ILogger<TestManager> logger)
        {
            _configuration = configuration;
            _testProvider = testProvider;
            _redisCacheService = redisCacheService;
            _logger = logger;
        }

        public async Task<ResponseModel<SettingModel>> Regions(SettingModel model)
        {
            var settingModel = new SettingModel();
            settingModel.RegionId = model.RegionId;
            settingModel.LocationCoordinates = model.LocationCoordinates;
            settingModel.DisasterTypes = model.DisasterTypes;

            // set setting region
            
            await SetCache("setting_" + model.RegionId, settingModel, cacheDuration);

            var rsponse = new ResponseModel<SettingModel>
            {
                data = settingModel
            };

            return rsponse;
        }

        public async Task<ResponseModel<SettingAlertModel>> AlertSettings(SettingAlertModel model)
        {
            var settingAlertModel = new SettingAlertModel();
            settingAlertModel.RegionId = model.RegionId;
            settingAlertModel.DisasterTypes = model.DisasterTypes;
            settingAlertModel.ThresholdScore = model.ThresholdScore;

            // set setting alert
            var cacheDurationAlertSetting = TimeSpan.FromSeconds(300);
            await SetCache("settingAlert_" + model.RegionId, settingAlertModel, cacheDurationAlertSetting);

            var rsponse = new ResponseModel<SettingAlertModel>
            {
                data = settingAlertModel
            };

            return rsponse;
        }

        public async Task<List<DisasterRisksModel>> DisasterRisks()
        {
            var modelData = new List<DisasterRisksModel>();

            var region = "1";
            var valueRegion = GetCache($"setting_{region}")?.Result ?? "";

            var valueRegionAllKeys = _redisCacheService.GetAll("setting_*").Result;
            if (!valueRegionAllKeys.Any())
            {
                throw new DisastersException.NotFound("setting data not fount");
            }

            var valueEartvhquakesAllKeys = _redisCacheService.GetAll("earthquakes_*").Result;
            if (!valueEartvhquakesAllKeys.Any())
            {
                throw new DisastersException.NotFound("disaster earthquakes data not fount");
            }

            var response = RiskCalculation(valueEartvhquakesAllKeys, valueRegionAllKeys).Result ?? new List<DisasterRisksModel>();

            return response;
        }

        public async Task<string> AlertsSend(int? regionId)
        {
            var valueEartvhquakesAllKeys = _redisCacheService.GetAll("earthquakes_*").Result;
            if (!valueEartvhquakesAllKeys.Any())
            {
                throw new DisastersException.NotFound("disaster earthquakes data not fount");
            }

            var listFeature = new List<Feature>();
            foreach (var item in valueEartvhquakesAllKeys)
            {
                var getValue = GetCache(item)?.Result ?? "";
                var modelValue = JsonConvert.DeserializeObject<Feature>(getValue);
                listFeature.Add(modelValue);
            }

            var query = listFeature.Where(a => a.properties.mag >= 1m);

            if (regionId.HasValue && regionId > 0)
            {
                query = query.Where(a => Convert.ToInt16(a.properties.RegionId) == regionId);
            }

            if (!query.Any())
            {
                throw new DisastersException.NoContent("data disaster earthquakes no content");
            }

            var data = query.ToList();
            
            foreach (var item in data)
            {
                // check ThresholdCheck 
                var isSend = ThresholdCheck(item).Result;
                if (isSend)
                {
                    // send email or alert bla bla bla
                    SendEmail(item);

                    // set AlertTriggered = true
                    item.properties.AlertTriggered = true;
                    item.properties.AlertMessage = "Alert msgggg !!!!!";
                    item.properties.Timestamp = (DateTime.Now).ToString("yyyyMMddHHmmssffff");

                    var key = "earthquakes_" + item.properties.RegionId + item.properties.time;

                    await RemoveCache(key);
                    await SetCache(key, item, cacheDuration);
                }
            }

            return "success";
        }

        public async Task<List<DisasterRisksModel>> Alerts()
        {
            var valueEartvhquakesAllKeys = _redisCacheService.GetAll("earthquakes_*").Result;
            if (!valueEartvhquakesAllKeys.Any())
            {
                throw new DisastersException.NotFound("disaster earthquakes data not fount");
            }

            var listFeature = new List<Feature>();
            foreach (var item in valueEartvhquakesAllKeys)
            {
                var getValue = GetCache(item)?.Result ?? "";
                var modelValue = JsonConvert.DeserializeObject<Feature>(getValue);
                listFeature.Add(modelValue);
            }

            var data = listFeature.Where(a => a.properties.AlertTriggered)
            .GroupBy(a => new
            {
                RegionId = a.properties.RegionId,
                DisasterTypes = a.properties.DisasterTypes,
            })
            .Select(a => new DisasterRisksModel
            {
                RegionId = a.Key.RegionId,
                DisasterTypes = a.Key.DisasterTypes,
                RiskScore = a.FirstOrDefault().properties.mag.ToString(),
                RiskLevel = CheckLevel(Convert.ToDecimal(a.FirstOrDefault()?.properties.mag ?? 0)).Key,
                RiskLevelName = CheckLevel(Convert.ToDecimal(a.FirstOrDefault()?.properties.mag ?? 0)).Value,
                AlertTriggered = a.FirstOrDefault().properties.AlertTriggered,
                AlertMessage = a.FirstOrDefault().properties.AlertMessage,
                Timestamp = a.FirstOrDefault().properties.Timestamp,
            })
            .ToList();

            // save in database 

            return data;
        }

        // get data external api
        public async Task<bool> GetDataDisaster()
        {
            try
            {
                // remove old cache
                var eartvhquakesAllKeys = _redisCacheService.GetAll("earthquakes_*").Result;
                foreach (var key in eartvhquakesAllKeys)
                {
                    await RemoveCache(key);
                }

                _logger.LogInformation("Job starting ...");

                var _client = new RestClient("https://earthquake.usgs.gov/");
                var _request = new RestRequest("earthquakes/feed/v1.0/summary/all_hour.geojson", Method.Get);

                var response = _client.Execute(_request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation("application Execute data");

                    var listResponse = JsonConvert.DeserializeObject<JobDataModel>(response.Content);

                    var count = 1;

                    var data = listResponse.features.GroupBy(a => a.properties.place.Split(',')[a.properties.place.Split(',').Length - 1]);
                    foreach (var item in data)
                    {
                        var prop = item.FirstOrDefault().properties;
                        var mag = prop.mag;
                        var place = prop.place;
                        var region = place.Split(',')[place.Split(',').Length - 1];
                        var time = prop.time;       // timestamp GMT+07:00
                        var update = prop.updated;  // timestamp GMT+07:00

                        var levelData = CheckLevel(mag ?? 0);

                        prop.Level = levelData.Key;
                        prop.RegionId = count.ToString();
                        prop.RegionName = region;
                        prop.LocationCoordinates = item.FirstOrDefault().geometry.coordinates[1].ToString() 
                                           + ";" + item.FirstOrDefault().geometry.coordinates[2].ToString();

                        prop.DisasterTypes = "earthquakes";

                        var IsDontHaveKeyData = string.IsNullOrEmpty(GetCache($"earthquakes_{prop.RegionId}" + time.ToString()).Result) ? true : false;
                        if (IsDontHaveKeyData)
                        {
                            await SetCache($"earthquakes_{prop.RegionId}" + time.ToString(), item.FirstOrDefault(), cacheDuration);
                        }

                        count++;
                    }
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                throw new DisastersException.NotFound($"get data external api error : {ex}");
            }

            return true;

        }

        public async Task<string> GetCache(string key)
        {
            try
            {
                var data = _redisCacheService.GetCachedData(key);

                Log.Logger.Information($"GetCache key:{key} data:{data}");

                return data;
            }
            catch
            {

                return null;
            }
        }

        private async Task<bool> SetCache<T>(string key, T model, TimeSpan cacheDuration)
        {
            try
            {
                var value = JsonConvert.SerializeObject(model);

                _redisCacheService.SetCachedData(key, value, cacheDuration);

                return true;
            }
            catch
            {

                return false;
            }

        }

        public async Task<bool> RemoveCache(string key)
        {
            try
            {
                _redisCacheService.Remove(key);

                return true;
            }
            catch
            {

                return false;
            }

            return true;
        }


        private KeyValuePair<string, string> CheckLevel(decimal mag)
        {
            if (mag == 0)
            {
                return new KeyValuePair<string, string>("", "");
            }

            try
            {
                var level = 0;
                var levelName = "-";

                if (mag < 1)
                {
                    level = 1;
                    levelName = "normal";
                }
                else if (mag > 1 && mag <= 2.5m)
                {
                    level = 2;
                    levelName = "little";
                }
                else if (mag > 2.5m && mag <= 4.5m)
                {
                    level = 3;
                    levelName = "medium";
                }
                else if (mag > 4.5m)
                {
                    level = 4;
                    levelName = "high";
                }


                return new KeyValuePair<string, string>(level.ToString(), levelName);

            }
            catch
            {

                return new KeyValuePair<string, string>("", "");
            }
        }

        private async Task<List<DisasterRisksModel>> RiskCalculation(List<string> feature, List<string> setting)
        {
            try
            {
                var modelSetting = new List<SettingModel>();
                foreach (var item in setting)
                {
                    var getValue = GetCache(item)?.Result ?? "";

                    var settingData = JsonConvert.DeserializeObject<SettingModel>(getValue);
                    modelSetting.Add(settingData);
                }

                var disasterRisksModel = new List<DisasterRisksModel>();

                var regionIdInSetting = modelSetting.Select(a => a.RegionId).ToList();

                foreach (var item in feature)
                {
                    var getValue = GetCache(item)?.Result ?? "";
                    var modelValue = JsonConvert.DeserializeObject<Feature>(getValue);

                    var isMapRegionId = (regionIdInSetting).Contains(modelValue?.properties?.RegionId);
                    if (isMapRegionId)
                    {
                        disasterRisksModel.Add(new DisasterRisksModel
                        {
                            RegionId = modelValue.properties.RegionId,
                            RegionName = modelValue.properties.RegionName,
                            DisasterTypes = modelValue.properties.DisasterTypes,
                            RiskScore = modelValue.properties.mag.ToString(),
                            RiskLevel = CheckLevel(Convert.ToDecimal(modelValue.properties.mag)).Key,
                            RiskLevelName = CheckLevel(Convert.ToDecimal(modelValue.properties.mag)).Value,
                            AlertTriggered = modelValue.properties.AlertTriggered,
                            AlertMessage = modelValue.properties.AlertMessage ?? "",
                        });
                    }
                   
                }

                return disasterRisksModel;
            }
            catch
            {

                return null;
            }
        }

        private async Task<bool> ThresholdCheck(Feature model)
        {
            try
            {
                var valueAlertRegionAllKeys = _redisCacheService.GetAll("settingAlert_*").Result;
                if (!valueAlertRegionAllKeys.Any())
                {
                    return true;
                }


                return true;
            }
            catch
            {

                return false;
            }
        }

        private async Task SendEmail(Feature data)
        {
            var _lstEmail = new List<string>();
            _lstEmail.Add("maxjedsada11@gmail.com");

            var host = "smtp.gmail.com";
            var port = "587";
            var user = "maxjedsada1@gmail.com";
            var pass = "oafqhftwwovzjweq";
            var fromEmail = "maxjedsada1@gmail.com";

            var smtpConfig = new SmtpClient
            {
                Host = host,
                Port = Convert.ToInt32(port),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(user, pass)
            };

            var subject = "test";
            var bodyHtml = "test detail feature region id : " + data.properties.RegionId
                    + " region name : " + data.properties.RegionName
                    + " mag value : " + data.properties.mag
                    + " level : " + data.properties.Level;

            foreach (var _mail in _lstEmail)
            {
                try
                {
                    var message = new MailMessage(fromEmail, _mail)
                    {
                        Subject = subject,
                        Body = bodyHtml,
                        IsBodyHtml = true,
                        BodyEncoding = System.Text.Encoding.UTF8,
                        SubjectEncoding = System.Text.Encoding.UTF8,
                    };

                    smtpConfig.Send(message);
                }
                catch { }
            }
        }


    }
}
