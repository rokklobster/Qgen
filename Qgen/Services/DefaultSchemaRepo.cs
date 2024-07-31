using Qgen.Contracts.Services;

namespace Qgen.Services
{
    public class DefaultSchemaRepo : SchemaRepo
    {
        private readonly Dictionary<Type, object> schemata = new();

        protected void Register<T>(Schema<T> s) where T : class => schemata[typeof(T)] = s;

        public Schema<T>? Get<T>() => schemata.TryGetValue(typeof(T), out var schema) && schema is Schema<T> s
                ? s : null;

        protected virtual void ManualRegister()
        {
        }
    }
}
