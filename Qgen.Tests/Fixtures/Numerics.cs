using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Qgen.Contracts.Models;
using Qgen.Tests.System;

namespace Qgen.Tests.Fixtures
{
    public class Numerics : IClassFixture<InMemoryDbFixtureBase>
    {
        private readonly InMemoryDbFixtureBase fixture;

        public Numerics(InMemoryDbFixtureBase fixture)
        {
            this.fixture = fixture;

            var db = fixture.Dependencies.Db;
            db.EntitiesF.RemoveRange(db.EntitiesF);
            db.AddRange(PrepareData());
            db.SaveChanges();
        }

        [Theory]
        [MemberData(nameof(SimpleOperatorsCases))]
        public Task TestSimpleOperators(Operation op, string arg,
            Func<TestDb, IQueryable<TestEntityFluent>> expected) =>
            fixture.RunTest<TestEntityFluent>(async (db, qb, _) =>
            {
                var res = qb.GetFrom(
                    new Query(Filters: new FilterComposition(
                        CompositionOp.And,
                        filters: new[] { new Filter(op, nameof(TestEntityFluent.Amount), arg) }
                    )));

                var resEntries = (await res.ToListAsync()).Select(Simplify).ToArray();
                var expEntries = (await expected(db).OrderBy(x => x.Id).ToListAsync()).Select(Simplify).ToArray();

                resEntries.Should().BeEquivalentTo(expEntries);
            });

        [Theory]
        [MemberData(nameof(SimpleAbsOperatorsCases))]
        public Task TestSimpleOperatorsForTransformedValue(Operation op, string arg,
            Func<TestDb, IQueryable<TestEntityFluent>> expected) =>
            fixture.RunTest<TestEntityFluent>(async (db, qb, _) =>
            {
                var res = qb.GetFrom(
                    new Query(Filters: new FilterComposition(
                        CompositionOp.And,
                        filters: new[] { new Filter(op, nameof(TestEntityFluent.Abs), arg) }
                    )));

                var resEntries = (await res.ToListAsync()).Select(Simplify).ToArray();
                var expEntries = (await expected(db).OrderBy(x => x.Id).ToListAsync()).Select(Simplify).ToArray();

                resEntries.Should().BeEquivalentTo(expEntries);
            });

        [Theory]
        [MemberData(nameof(ErroneousSimpleOperatorsCases))]
        public Task TestSimpleOperatorsForErrors(Operation op, string arg) =>
            fixture.RunTest<TestEntityFluent>((_, qb, _) =>
            {
                new Action(() => qb.GetFrom(
                        new Query(Filters: new FilterComposition
                        (
                            CompositionOp.And,
                            filters: new[] { new Filter(op, nameof(TestEntityFluent.Amount), arg) }
                        ))))
                    .Should().Throw<Exception>();

                return Task.CompletedTask;
            });

        private static IEnumerable<object[]> SimpleOperatorsCases()
        {
            var arr = new[] { 12, 13, 14 };
            var arrN = new[] { -12, -13, -14 };
            return new[]
            {
                SimpleCase(Operation.Eq, "12", d => d.EntitiesF.Where(x => x.Amount == 12)),
                SimpleCase(Operation.Neq, "12", d => d.EntitiesF.Where(x => x.Amount != 12)),
                SimpleCase(Operation.Gr, "12", d => d.EntitiesF.Where(x => x.Amount > 12)),
                SimpleCase(Operation.GrEq, "12", d => d.EntitiesF.Where(x => x.Amount >= 12)),
                SimpleCase(Operation.Lt, "12", d => d.EntitiesF.Where(x => x.Amount < 12)),
                SimpleCase(Operation.LtEq, "12", d => d.EntitiesF.Where(x => x.Amount <= 12)),
                SimpleCase(Operation.In, "[12,13,14]", d => d.EntitiesF.Where(x => arr.Contains(x.Amount!.Value))),
                SimpleCase(Operation.NotIn, "[12,13,14]", d => d.EntitiesF.Where(x => !arr.Contains(x.Amount!.Value))),
                SimpleCase(Operation.InRange, "[12,14]", d => d.EntitiesF.Where(x => 12 <= x.Amount && x.Amount <= 14)),
                SimpleCase(Operation.NotInRange, "[12,14]",
                    d => d.EntitiesF.Where(x => !(12 <= x.Amount && x.Amount <= 14))),
                SimpleCase(Operation.Eq, "-12", d => d.EntitiesF.Where(x => x.Amount == -12)),
                SimpleCase(Operation.Neq, "-12", d => d.EntitiesF.Where(x => x.Amount != -12)),
                SimpleCase(Operation.Gr, "-12", d => d.EntitiesF.Where(x => x.Amount > -12)),
                SimpleCase(Operation.GrEq, "-12", d => d.EntitiesF.Where(x => x.Amount >= -12)),
                SimpleCase(Operation.Lt, "-12", d => d.EntitiesF.Where(x => x.Amount < -12)),
                SimpleCase(Operation.LtEq, "-12", d => d.EntitiesF.Where(x => x.Amount <= -12)),
                SimpleCase(Operation.In, "[-12,-13,-14]", d => d.EntitiesF.Where(x => arrN.Contains(x.Amount!.Value))),
                SimpleCase(Operation.NotIn, "[-12,-13,-14]",
                    d => d.EntitiesF.Where(x => !arrN.Contains(x.Amount!.Value))),
                SimpleCase(Operation.InRange, "[-14,-12]",
                    d => d.EntitiesF.Where(x => -14 <= x.Amount && x.Amount <= -12)),
                SimpleCase(Operation.NotInRange, "[-14,-12]",
                    d => d.EntitiesF.Where(x => !(-14 <= x.Amount && x.Amount <= -12))),
                SimpleCase(Operation.In, "[]", d => d.EntitiesF.Where(x => false)),
                SimpleCase(Operation.NotIn, "[]", d => d.EntitiesF.Where(x => true)),
            };
        }

        private static IEnumerable<object[]> SimpleAbsOperatorsCases()
        {
            var arr = new[] { 12, 13, 14 };
            var arrN = new[] { -12, -13, -14 };
            return new[]
            {
                SimpleCase(Operation.Eq, "12", d => d.EntitiesF.Where(x => Math.Abs(x.Abs.Value) == 12)),
                SimpleCase(Operation.Neq, "12", d => d.EntitiesF.Where(x => Math.Abs(x.Abs.Value) != 12)),
                SimpleCase(Operation.Gr, "12", d => d.EntitiesF.Where(x => Math.Abs(x.Abs.Value) > 12)),
                SimpleCase(Operation.GrEq, "12", d => d.EntitiesF.Where(x => Math.Abs(x.Abs.Value) >= 12)),
                SimpleCase(Operation.Lt, "12", d => d.EntitiesF.Where(x => Math.Abs(x.Abs.Value) < 12)),
                SimpleCase(Operation.LtEq, "12", d => d.EntitiesF.Where(x => Math.Abs(x.Abs.Value) <= 12)),
                SimpleCase(Operation.In, "[12,13,14]",
                    d => d.EntitiesF.Where(x => arr.Contains(Math.Abs(x.Abs.Value)))),
                SimpleCase(Operation.NotIn, "[12,13,14]",
                    d => d.EntitiesF.Where(x => !arr.Contains(Math.Abs(x.Abs.Value)))),
                SimpleCase(Operation.InRange, "[12,14]",
                    d => d.EntitiesF.Where(x => 12 <= Math.Abs(x.Abs.Value) && Math.Abs(x.Abs.Value) <= 14)),
                SimpleCase(Operation.NotInRange, "[12,14]",
                    d => d.EntitiesF.Where(x => !(12 <= Math.Abs(x.Abs.Value) && Math.Abs(x.Abs.Value) <= 14))),
                SimpleCase(Operation.Eq, "-12", d => d.EntitiesF.Where(x => Math.Abs(x.Abs.Value) == -12)),
                SimpleCase(Operation.Neq, "-12", d => d.EntitiesF.Where(x => Math.Abs(x.Abs.Value) != -12)),
                SimpleCase(Operation.Gr, "-12", d => d.EntitiesF.Where(x => Math.Abs(x.Abs.Value) > -12)),
                SimpleCase(Operation.GrEq, "-12", d => d.EntitiesF.Where(x => Math.Abs(x.Abs.Value) >= -12)),
                SimpleCase(Operation.Lt, "-12", d => d.EntitiesF.Where(x => Math.Abs(x.Abs.Value) < -12)),
                SimpleCase(Operation.LtEq, "-12", d => d.EntitiesF.Where(x => Math.Abs(x.Abs.Value) <= -12)),
                SimpleCase(Operation.In, "[-12,-13,-14]",
                    d => d.EntitiesF.Where(x => arrN.Contains(Math.Abs(x.Abs.Value)))),
                SimpleCase(Operation.NotIn, "[-12,-13,-14]",
                    d => d.EntitiesF.Where(x => !arrN.Contains(Math.Abs(x.Abs.Value)))),
                SimpleCase(Operation.InRange, "[-14,-12]",
                    d => d.EntitiesF.Where(x => -14 <= Math.Abs(x.Abs.Value) && Math.Abs(x.Abs.Value) <= -12)),
                SimpleCase(Operation.NotInRange, "[-14,-12]",
                    d => d.EntitiesF.Where(x => !(-14 <= Math.Abs(x.Abs.Value) && Math.Abs(x.Abs.Value) <= -12))),
                SimpleCase(Operation.In, "[]", d => d.EntitiesF.Where(x => false)),
                SimpleCase(Operation.NotIn, "[]", d => d.EntitiesF.Where(x => true)),
            };
        }

        private static IEnumerable<object[]> ErroneousSimpleOperatorsCases()
        {
            var arr = new[] { 12, 13, 14 };
            return new[]
            {
                SimpleCase(Operation.Contains, "12"),
                SimpleCase(Operation.DoesNotContain, "12"),
                SimpleCase(Operation.Eq, "non-int"),
                SimpleCase((Operation)999, "12"),
            };
        }

        private static object[] SimpleCase(Operation o, string s, Func<TestDb, IQueryable<TestEntityFluent>>? e = null)
            => e is null ? new object[] { o, s } : new object[] { o, s, e };

        private static string Simplify(TestEntityFluent t) => $"{t.Amount}";

        private static TestEntityFluent[] PrepareData() => Enumerable.Range(-32, 64)
            .Select(i => new TestEntityFluent { Id = Guid.NewGuid(), Amount = i, Abs = i })
            .ToArray();
    }
}