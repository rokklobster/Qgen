using Qgen.Contracts.Services;
using Qgen.Services;

namespace Qgen.Tests.System
{
    public abstract class DbFixtureBase : IAsyncLifetime
    {
        public Dependencies Dependencies { get; private set; }

        public Task RunTest<T>(Func<TestDb, QueryBuilder<T>, Dependencies, Task> test) where T : class
        {
            var builder = Dependencies.QueryBuilder<T>();
            return test(Dependencies.Db, builder, Dependencies);
        }

        public async Task DisposeAsync()
        {
            await Dependencies.Db.Database.EnsureDeletedAsync();
            await OnDispose();
        }

        public async Task InitializeAsync()
        {
            var db = await GetDb();
            await db.Database.EnsureCreatedAsync();
            Dependencies = new(db);
        }

        protected abstract Task<TestDb> GetDb();

        protected virtual Task OnDispose() => Task.CompletedTask;
    }
}
