namespace Qgen.Contracts.Models
{
    public class Filter
    {
        public Filter(Operation op, string field, string arg)
        {
            Op = op;
            Field = field;
            Arg = arg;
        }

        public Operation Op { get; }
        public string Field { get; }
        public string Arg { get; }
    }
}
