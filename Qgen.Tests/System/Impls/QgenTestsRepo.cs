using Qgen.Services;
using Qgen.Tests.System.DB;

namespace Qgen.Tests.System.Impls
{
    public partial class QgenTestsRepo: DefaultSchemaRepo
    {
        protected override void ManualRegister()
        {
            Register(TestEntityFluent.Schema);
        }
    }
}
