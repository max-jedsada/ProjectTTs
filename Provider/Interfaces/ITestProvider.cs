
using Manager.Entities;

namespace Provider.Interfaces
{
    public interface ITestProvider
    {
        IQueryable<TimeSetting> Get();
        TimeSetting Get(int id);

    }
}
