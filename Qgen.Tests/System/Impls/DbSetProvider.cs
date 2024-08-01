using Qgen.Contracts.Services;
using Qgen.Tests.System.DB;

namespace Qgen.Tests.System.Impls
{
    public class DbSetProvider : SetProvider
    {
        private readonly TestDb db;

        public DbSetProvider(TestDb db)
        {
            this.db = db;
        }

        public IQueryable<T> Get<T>() where T : class => db.Set<T>();
    }
}
