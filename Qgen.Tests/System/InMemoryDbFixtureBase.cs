using Microsoft.EntityFrameworkCore;

namespace Qgen.Tests.System
{
    public class InMemoryDbFixtureBase : DbFixtureBase
    {
        protected override Task<TestDb> GetDb() =>
            Task.FromResult(new TestDb((o, s) => o.UseInMemoryDatabase(s)));
    }
}
