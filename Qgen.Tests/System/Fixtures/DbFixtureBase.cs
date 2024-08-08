using Qgen.Services;
using Qgen.Tests.System.DB;
using Qgen.Tests.System.Impls;

namespace Qgen.Tests.System.Fixtures;

public abstract class DbFixtureBase : IAsyncLifetime
{
    private readonly Dictionary<int, Func<Dependencies, Task>> callbacks = new();
    private int lastKey = 0;

    public async Task<Dependencies> GetDependencies()
    {
        var db = await GetDb();
        await db.Database.EnsureCreatedAsync();
        return new(db);
    }

    public async Task RunTest<T>(Func<TestDb, QueryBuilder<T>, Dependencies, Task> test) where T : class
    {
        var deps = await GetDependencies();

        foreach (var cb in callbacks.Values)
        {
            await cb(deps);
        }

        var builder = deps.QueryBuilder<T>();
        await test(deps.Db, builder, deps);
    }

    public virtual Task DisposeAsync() => Task.CompletedTask;

    public abstract Task InitializeAsync();

    protected abstract Task<TestDb> GetDb();

    public Action RegisterPreTestCallback(Func<Dependencies, Task> callback)
    {
        var key = Interlocked.Increment(ref lastKey);

        callbacks.Add(key, callback);
        return () => callbacks.Remove(key);
    }
}
