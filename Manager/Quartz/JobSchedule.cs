using Manager.Interfaces;
using Project.Manager.Redis;
using Quartz;

namespace Manage.Quartz.Job;

public class JobSchedule : IJob
{
    private ITestManager _testManager;

    public JobSchedule(ITestManager testManager, IRedisCacheService redisCacheService)
    {
        _testManager = testManager;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var isSuccess = await _testManager.GetDataDisaster();

        await Task.CompletedTask;
    }

    

}
