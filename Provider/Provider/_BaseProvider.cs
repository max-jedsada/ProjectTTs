
using Microsoft.Extensions.Configuration;
using Project.Context;

namespace Base.Provider
{
    public abstract class _BaseProvider
    {
        protected ProjectContext _db;
        private readonly IConfiguration _configuration;

        public _BaseProvider(ProjectContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

    }
}
