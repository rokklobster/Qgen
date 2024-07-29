namespace Qgen.Contracts.Models
{
    public enum Operation
    {
        Eq = 0,
        Neq = 1,
        Gr, GrEq,
        Lt, LtEq,
        In, NotIn,
        Contains, DoesNotContain,
        InRange, NotInRange
    }
}
