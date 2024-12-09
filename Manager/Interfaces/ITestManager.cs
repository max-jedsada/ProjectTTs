
using Manager.Model;
using Project.Manager.ResponseModels;

namespace Manager.Interfaces
{
    public interface ITestManager
    {
        Task<ResponseModel<SettingModel>> Regions(SettingModel model);
        Task<ResponseModel<SettingAlertModel>> AlertSettings(SettingAlertModel model);
        Task<List<DisasterRisksModel>> DisasterRisks();
        Task<string> AlertsSend(int? regionId);
        Task<List<DisasterRisksModel>> Alerts();

        Task<bool> GetDataDisaster();

        Task<string> GetCache(string key);

        Task<bool> RemoveCache(string key);

    }
}
