using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Qgen.Contracts.Models;
using Qgen.Tests.System;
using Qgen.Tests.System.DB;
using Qgen.Tests.System.Fixtures;
using Qgen.Tests.System.Impls;

namespace Qgen.Tests.Fixtures
{
    public class Strings : IClassFixture<InMemoryDbFixtureBase>
    {
        private readonly InMemoryDbFixtureBase fixture;

        public Strings(InMemoryDbFixtureBase fixture)
        {
            this.fixture = fixture;

            var db = fixture.Dependencies.Db;
            db.EntitiesF.RemoveRange(db.EntitiesF);
            db.AddRange(PrepareData());
            db.SaveChanges();
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(SimpleOperatorsCases))]
        public Task TestSimpleOperators(Operation op, string arg, Func<TestDb, IQueryable<TestEntityFluent>> expected) =>
            fixture.RunTest<TestEntityFluent>(async (db, qb, _) =>
            {
                var res = qb.GetFrom(
                    new Query(Filters: new FilterComposition
                    (
                        CompositionOp.And,
                        filters: new[] {
                        new Filter(op, nameof(TestEntityFluent.Name), arg) }
                    )));

                var resEntries = (await res.ToListAsync()).Select(Simplify).ToArray();
                var expEntries = (await expected(db).OrderBy(x => x.Id).ToListAsync()).Select(Simplify).ToArray();

                resEntries.Should().BeEquivalentTo(expEntries);
            });

        private static IEnumerable<object[]> SimpleOperatorsCases()
        {
            var arr = new[] { "000101", "000110", "000111" };
            var arrArg = JsonConvert.SerializeObject(arr);
            return new[]
            {
                SimpleCase(Operation.Eq, "000101", d => d.EntitiesF.Where(x=>x.Name == "000101")),
                SimpleCase(Operation.Neq, "000101", d => d.EntitiesF.Where(x=>x.Name != "000101")),
                SimpleCase(Operation.In, arrArg, d => d.EntitiesF.Where(x=>arr.Contains(x.Name))),
                SimpleCase(Operation.NotIn, arrArg, d => d.EntitiesF.Where(x=>!arr.Contains(x.Name))),
                SimpleCase(Operation.Contains, "101", d => d.EntitiesF.Where(x=>x.Name!.IndexOf("101")>=0)),
                SimpleCase(Operation.Contains, "\"\"", d => d.EntitiesF.Where(x=>true)),
                SimpleCase(Operation.DoesNotContain, "\"\"", d => d.EntitiesF.Where(x=>false)),
                SimpleCase(Operation.DoesNotContain, "101", d => d.EntitiesF.Where(x=> !x.Name!.Contains("101"))),
            };
        }

        private static string Simplify(TestEntityFluent t) => $"{t.Name}";

        private static object[] SimpleCase(Operation o, string s, Func<TestDb, IQueryable<TestEntityFluent>>? e = null)
            => e is null ? new object[] { o, s } : new object[] { o, s, e };

        private static TestEntityFluent[] PrepareData() => Enumerable.Range(0, 64)
                .Select(i => new TestEntityFluent { Id = Guid.NewGuid(), Name = Bits(i, 6) })
                .ToArray();

        private static string Bits(int i, int width) =>
            new(Enumerable.Range(0, width).Select(x => 1 << x).Reverse().Select(x => (i & x) == 0 ? '0' : '1').ToArray());
    }
}
