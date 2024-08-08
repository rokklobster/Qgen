﻿using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Npgsql.Replication.TestDecoding;
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

        [Theory]
        [MemberData(nameof(SearchCases))]
        public Task SearchCustomizationsWorkAsExpected(string search, string[]? include, Func<TestDb, IQueryable<TestEntityDecl>> expected) =>
            fixture.RunTest<TestEntityDecl>(async (db, qb, _) =>
            {
                var res = qb.GetFrom(
                    new Query(Include: include, SearchQuery: search));

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

        private static IEnumerable<object[]> SearchCases() =>
            new[] {
                SearchCase("e", new[]{ nameof(TestEntityDecl.Name)}, s => s.Where(x => x.Name.Contains("e"))),
                SearchCase("e", new[]{ nameof(TestEntityDecl.Code)}, s => s.Where(x => TestDb.GetMessageForCode(x.Code).Contains("e"))),
                SearchCase("e", null, s => s.Where(x => TestDb.GetMessageForCode(x.Code).Contains("e") || x.Name.Contains("e"))),
            };

        private static object[] SimpleCase(Operation op, string arg, Func<DbSet<TestEntityDecl>, IQueryable<TestEntityDecl>> t) =>
            new object[] { op, arg, (TestDb db) => t(db.EntitiesD) };

        private static object[] SearchCase(string arg, string[]? incl, Func<DbSet<TestEntityDecl>, IQueryable<TestEntityDecl>> t) =>
            new object[] { arg, incl, (TestDb db) => t(db.EntitiesD) };

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
