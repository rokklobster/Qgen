using Microsoft.EntityFrameworkCore;
using Qgen.Tests.System.DB;
using Squadron;

namespace Qgen.Tests.System.Fixtures;

public class SquadronDbFixtureBase : DbFixtureBase
{
    private readonly PostgreSqlResource resource = new();

    protected override async Task<TestDb> GetDb()
    {
        await resource.InitializeAsync();

        return new TestDb((b, s) => b.UseNpgsql(resource.GetConnectionString(s)));
    }

    protected override async Task OnDispose()
    {
        await resource.DisposeAsync();
    }
}
