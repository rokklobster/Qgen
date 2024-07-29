using Qgen.Contracts.Services;

namespace Qgen.Tests.System
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
