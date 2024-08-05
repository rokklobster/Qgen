#nullable disable

using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Qgen.Tests.System.DB;

public class TestDb : DbContext
{
    private readonly bool skipConfiguring;

    public static string DbId() => "tdb_" + Guid.NewGuid().ToString()[..6];

    public TestDb(Func<DbContextOptionsBuilder<TestDb>, string, DbContextOptionsBuilder> build, bool skipConfiguring = true)
        : base(build(new DbContextOptionsBuilder<TestDb>(), DbId()).Options)
    {
        this.skipConfiguring = skipConfiguring;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        if (skipConfiguring) return;

        modelBuilder.HasDbFunction(typeof(TestDb).GetMethod(nameof(GetMessageForCode), BindingFlags.Static | BindingFlags.Public, new[] { typeof(int) }))
            .HasName("get_code_msg");
    }

    public DbSet<TestEntityDecl> EntitiesD { get; set; }

    public DbSet<TestEntityFluent> EntitiesF { get; set; }

    public static string GetMessageForCode(int code) => throw new Exception("DB proxy");
}
