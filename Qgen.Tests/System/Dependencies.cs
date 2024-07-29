using Qgen.Contracts.Services;
using Qgen.Services;

namespace Qgen.Tests.System
{
    public class Dependencies
    {
        public TestDb Db { get; }

        public SetProvider SetProvider { get; }

        public SchemaRepo Repo { get; }

        public QueryBuilder<T> QueryBuilder<T>() where T : class => new(SetProvider, Repo);

        public Dependencies(TestDb db)
        {
            Db = db;
            SetProvider=new DbSetProvider(db);
            Repo = SchemaRepo.AppInstance ?? throw new NullReferenceException("Schema repo is not populated");
        }
    }
}
