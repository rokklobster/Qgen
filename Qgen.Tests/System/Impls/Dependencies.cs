using Qgen.Contracts.Services;
using Qgen.Services;
using Qgen.Tests.System.DB;

namespace Qgen.Tests.System.Impls
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
            Repo = Generated.Schema.QgenTestsRepo.Instance;
        }
    }
}
