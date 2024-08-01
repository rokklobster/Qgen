#nullable disable

using Microsoft.EntityFrameworkCore;

namespace Qgen.Tests.System.DB
{
    public class TestDb : DbContext
    {
        public static string DbId() => "tdb_"+ Guid.NewGuid().ToString()[..6];

        public TestDb(Func<DbContextOptionsBuilder<TestDb>, string, DbContextOptionsBuilder> build)
            : base(build(new DbContextOptionsBuilder<TestDb>(), DbId()).Options)
        {

        }

        public DbSet<TestEntityDecl> EntitiesD { get; set; }

        public DbSet<TestEntityFluent> EntitiesF { get; set; }
    }
}
