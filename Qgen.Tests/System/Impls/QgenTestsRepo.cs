using Qgen.Contracts.Services;
using Qgen.Services;
using Qgen.Tests.System;

namespace Qgen.Tests.Generated.Schema
{
    public partial class QgenTestsRepo: DefaultSchemaRepo
    {
        partial void ManualRegister()
        {
            Register(TestEntityFluent.Schema);
        }
    }
}
