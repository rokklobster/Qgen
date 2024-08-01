using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;
using static Qgen.Services.Expressions;

namespace Qgen.Tests.System.Impls;

public static class Transformers
{
    internal static Expression Abs(Expression p)
    {
        // already read the value of prop
        return WrapInNullCheck(p, p.Type,
            val => Call(
                GetMethod(typeof(Math), nameof(Math.Abs), m => m.GetParameters().SingleOrDefault(x => x.ParameterType == typeof(int)) is not null)!,
                val),
            () => Constant(0));
    }
}
