using Qgen.Services;

namespace Qgen.Tests.System
{
    internal class TestSchemaRepo: DefaultSchemaRepo
    {
        public TestSchemaRepo()
        {
            Register(TestEntityFluent.Schema);
        }
    }
}
