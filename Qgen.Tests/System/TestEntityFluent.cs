using Qgen.Contracts.Services;
using Qgen.Services;

namespace Qgen.Tests.System
{
    public class TestEntityFluent
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public int? Amount { get; set; }


        private static readonly Lazy<Schema<TestEntityFluent>> schema= new Lazy<Schema<TestEntityFluent>>(CreateSchema);
        public static Schema<TestEntityFluent> Schema => schema.Value;

        private static Schema<TestEntityFluent> CreateSchema()
        {
            var builder = new SchemaBuilder<TestEntityFluent>();

            builder.RegisterField(x => x.Id).EnableAll();
            builder.RegisterField(x => x.Name).EnableAll();
            builder.RegisterField(x => x.Amount).EnableAll();

            builder.AddDefaultOrderingLayer(x => x.Id, true);

            return builder.Result;
        }
    }
}
