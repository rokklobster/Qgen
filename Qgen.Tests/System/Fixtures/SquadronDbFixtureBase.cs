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

        TestDb db = new((b, s) => b.UseNpgsql(resource.GetConnectionString(s)));
        await db.Database.ExecuteSqlRawAsync(DbInit);
        return db;
    }

    protected override async Task OnDispose()
    {
        await resource.DisposeAsync();
    }

    private const string DbInit = @"
CREATE EXTENSION ""uuid-ossp"";

create table if not exists ""EntitiesD""(
  ""Id"" uuid primary key,
  ""Name"" nvarchar(63),
  ""Code"" int);
create table if not exists ""EntitiesF""(
  ""Id"" uuid primary key,
  ""Name"" nvarchar(63),
  ""Amount"" int,
  ""Abs"" int);

create table if not exists ""CodeMapping""(
  ""Code"",
  ""Message"" nvarchar(63)
);

create or replace function get_code_msg(code int)
returns text
language plpgsql
RETURNS NULL ON NULL INPUT
stable
parallel safe
as $$
declare ret text;
begin
    select ""Message"" into ret
    from ""CodeMapping""
    where abs(""Code"") = code
    limit 1;
    return ret;
end
$$;

insert into ""CodeMapping"" (""Code"", ""Message"")
values
  (0, 'zero'),
  (1, 'one'),
  (2, 'two'),
  (3, 'three'),
  (4, 'four'),
  (5, 'five'),
  (6, 'six'),
  (7, 'seven'),
  (8, 'eight'),
  (9, 'nine'),
  (10, 'ten'),
  (11, 'eleven'),
  (12, 'twelve'),
  (13, 'thirteen'),
  (14, 'fourteen'),
  (15, 'fifteen'),
  (16, 'sixteen'),
  (17, 'seventeen'),
  (18, 'eighteen'),
  (19, 'nineteen'),
  (20, 'twenty');
";
}
