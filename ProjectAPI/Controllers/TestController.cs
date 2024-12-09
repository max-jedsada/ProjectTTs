using Project.API.Area.Home.Custom.Controllers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Manager.Interfaces;
using Project.Utility;
using Manager.Model;
using Microsoft.AspNetCore.Authorization;
using Project.Manager.ResponseModels;

namespace Project.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route(BaseUrl)]
    public class TestController : _CustomController
    {
        private readonly ITestManager _testManager;

        public TestController(ITestManager testManager)
        {
            _testManager = testManager;
        }

        [HttpPost]
        [Route("regions")]
        [SwaggerResponse(StatusCodes.Status200OK, "manage regions")]
        public object Regions([FromBody] SettingModel model)
        {
            var preson = _testManager.Regions(model);
            var response = ResponseWrapper.Success(System.Net.HttpStatusCode.OK, preson);
            return response;
        }

        [HttpPost]
        [Route("alert-settings")]
        [SwaggerResponse(StatusCodes.Status200OK, "manage alert settings")]
        public object AlertSettings([FromBody] SettingAlertModel model)
        {
            var preson = _testManager.AlertSettings(model);
            var response = ResponseWrapper.Success(System.Net.HttpStatusCode.OK, preson.Result);
            return response;
        }

        [HttpGet]
        [Route("disaster-risks")]
        [SwaggerResponse(StatusCodes.Status200OK, "get disaster risks")]
        public object DisasterRisks()
        {
            var preson = _testManager.DisasterRisks();
            var response = ResponseWrapper.Success(System.Net.HttpStatusCode.OK, preson.Result);
            return response;
        }

        [HttpPost]
        [Route("alerts/send")]
        [SwaggerResponse(StatusCodes.Status200OK, "manage alerts send")]
        public object AlertsSend(int? regionId)
        {
            var preson = _testManager.AlertsSend(regionId);
            var response = ResponseWrapper.Success(System.Net.HttpStatusCode.OK, preson.Result ?? "");
            return response;
        }

        [HttpGet]
        [Route("alerts")]
        [SwaggerResponse(StatusCodes.Status200OK, "get alerts")]
        public object Alerts()
        {
            var preson = _testManager.Alerts();
            var response = ResponseWrapper.Success(System.Net.HttpStatusCode.OK, preson.Result);
            return response;
        }


        [HttpGet]
        [Route("runJob")]
        [SwaggerResponse(StatusCodes.Status200OK, "run Job")]
        public async Task<object> RunJob()
        {
            var isSuccess = await _testManager.GetDataDisaster();
            var response = ResponseWrapper.Success(System.Net.HttpStatusCode.OK, isSuccess ? "true" : "false");
            return response;
        }

        [HttpGet]
        [Route("getByKey")]
        [SwaggerResponse(StatusCodes.Status200OK, "get cache")]
        public object getByKey(string key)
        {
            var preson = _testManager.GetCache(key);
            var response = ResponseWrapper.Success(System.Net.HttpStatusCode.OK, preson.Result);
            return response;
        }

    }
}