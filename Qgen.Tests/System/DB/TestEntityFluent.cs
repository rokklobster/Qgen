using Qgen.Contracts.Services;
using Qgen.Services;
using Qgen.Tests.System.Impls;

namespace Qgen.Tests.System.DB
{
    public class TestEntityFluent
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public int? Amount { get; set; }

        public int? Abs { get; set; }

        private static readonly Lazy<Schema<TestEntityFluent>> schema= new Lazy<Schema<TestEntityFluent>>(CreateSchema);
        public static Schema<TestEntityFluent> Schema => schema.Value;

        private static Schema<TestEntityFluent> CreateSchema()
        {
            var builder = new SchemaBuilder<TestEntityFluent>();

            builder.RegisterField(x => x.Id).EnableAll();
            builder.RegisterField(x => x.Name).EnableAll();
            builder.RegisterField(x => x.Amount).EnableAll();
            builder.RegisterField(x => x.Abs).EnableFiltering(Transformers.Abs);

            builder.AddDefaultOrderingLayer(x => x.Id, true);

            return builder.Result;
        }
    }
}
