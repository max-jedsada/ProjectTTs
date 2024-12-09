using Project.Provider.Exception;
using Microsoft.Extensions.Configuration;
using Base.Provider;
using Provider.Interfaces;
using Project.Context;
using Manager.Entities;

namespace Provider.Services
{
    public class TestProvider : _BaseProvider, ITestProvider
    {
        private readonly IConfiguration _configuration;

        public TestProvider(ProjectContext db, IConfiguration configuration) : base(db, configuration)
        {
        }

        public IQueryable<TimeSetting> Get()
        {
            var data = _db.TimeSettings.AsQueryable();
            if (data is null)
            {
                throw new TimeSettingException.NotFound();
            }

            return data;
        }

        public TimeSetting Get(int id)
        {
            var persons = _db.TimeSettings.FirstOrDefault(a => a.Id == id);
            if (persons is null)
            {
                throw new TimeSettingException.NoContent(id);
            }

            return persons;
        }

    }
}
