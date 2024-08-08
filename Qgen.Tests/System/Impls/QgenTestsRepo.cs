using Qgen.Services;
using Qgen.Tests.System.DB;

namespace Qgen.Tests.Generated.Schema;

public partial class QgenTestsRepo: DefaultSchemaRepo
{
    protected override void ManualRegister()
    {
        Register(TestEntityFluent.Schema);
    }
}
