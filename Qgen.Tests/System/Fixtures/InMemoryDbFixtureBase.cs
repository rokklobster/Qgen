using Microsoft.EntityFrameworkCore;
using Qgen.Tests.System.DB;

namespace Qgen.Tests.System.Fixtures;

public class InMemoryDbFixtureBase : DbFixtureBase
{
    public override Task InitializeAsync() => Task.CompletedTask;

    protected override Task<TestDb> GetDb() =>
        Task.FromResult(new TestDb((o, s) => o.UseInMemoryDatabase(s)));
}
