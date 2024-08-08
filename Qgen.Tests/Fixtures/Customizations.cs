using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Qgen.Contracts.Models;
using Qgen.Tests.System.DB;
using Qgen.Tests.System.Fixtures;
using Qgen.Tests.System.Impls;

namespace Qgen.Tests.Fixtures
{
    public class Customizations : IClassFixture<SquadronDbFixtureBase>, IDisposable
    {
        private readonly SquadronDbFixtureBase fixture;
        private readonly Action unregister;

        public Customizations(SquadronDbFixtureBase fixture)
        {
            this.fixture = fixture;

            unregister = fixture.RegisterPreTestCallback(async deps =>
            {
                var db = deps.Db;
                db.EntitiesD.RemoveRange(db.EntitiesD);
                await db.AddRangeAsync(PrepareData());
                await db.SaveChangesAsync();
            });
        }

        [Theory]
        [MemberData(nameof(FilteringCases))]
        public Task FilterCustomizationWorksAsExpected(Operation op, string arg, Func<TestDb, IQueryable<TestEntityDecl>> expected) =>
            fixture.RunTest<TestEntityDecl>(async (db, qb, _) =>
            {
                var res = qb.GetFrom(
                    new Query(Filters: new FilterComposition(
                        CompositionOp.And,
                        filters: new[] { new Filter(op, nameof(TestEntityDecl.Code), arg) }
                )));

                var resEntries = (await res.ToListAsync()).Select(Simplify).ToArray();
                var expEntries = (await expected(db).OrderBy(x => x.Id).ToListAsync()).Select(Simplify).ToArray();

                resEntries.Should().BeEquivalentTo(expEntries);
            });

        private static IEnumerable<object[]> FilteringCases()
        {
            return new[] {
                SimpleCase(Operation.Eq, "12", set => set.Where(x => Math.Abs(x.Code) == 12))
            };
        }

        private static object[] SimpleCase(Operation op, string arg, Func<DbSet<TestEntityDecl>, IQueryable<TestEntityDecl>> t) =>
            new object[] { op, arg, (TestDb db) => t(db.EntitiesD) };

        private static string Simplify(TestEntityDecl t) => $"{t.Code} | {t.Name}";

        private static IEnumerable<TestEntityDecl> PrepareData()
        {
            var names = new[] { "one", "two", "three", "four" };
            return Enumerable.Range(-20, 41)
                .SelectMany(x => names.Select(n => (x, n)))
                .Select(x => new TestEntityDecl { Id = Guid.NewGuid(), Code = x.x, Name = x.n });
        }

        public void Dispose()
        {
            unregister();
        }
    }
}
