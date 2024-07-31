using Newtonsoft.Json;
using Qgen.Contracts.Models;
using Qgen.Contracts.Services;
using System.Linq.Expressions;
using static Qgen.Services.Expressions;
using static System.Linq.Expressions.Expression;

namespace Qgen.Services.Internal
{
    internal static class FilterBuilder
    {
        internal static Expression<Func<T, bool>> Build<T>(FilterComposition? filters, Schema<T> schema)
        {
            if (filters is null) return t => true;

            var p = Parameter(typeof(T), "t");

            return Lambda<Func<T, bool>>(GetExpressionForBracket(filters, schema, p), p);
        }

        internal static Expression GetExpressionForBracket<T>(FilterComposition f, Schema<T> s, ParameterExpression p)
        {
            Func<Expression, Expression, Expression> aggr = f.Op switch
            {
                CompositionOp.And => And,
                CompositionOp.Or => Or,
                _ => throw new ArgumentOutOfRangeException(nameof(f.Op), "Composition operation is invalid - use either And or Or"),
            };

            var exprs = f.Compositions.OrEmpty().Select(c => GetExpressionForBracket(c, s, p))
                .Concat(f.Filters.OrEmpty().Select(x => GetExpressionForFilter(x, s, p)));

            return exprs.WhereNotNull().IfEmpty(True).Aggregate(aggr);
        }

        internal static Expression? GetExpressionForFilter<T>(Filter f, Schema<T> s, ParameterExpression p)
        {
            Func<ParameterExpression, Expression>? t = s.TryGetFor(Target.Filter, f.Field);
            // todo: fail if not found?
            if (t is null) return null;

            var prop = t(p);
            var type = prop.Type;
            var cleanType = UnnullifyType(type);
            return WrapInNullCheck(prop, type,
                x => BuildFilterExpression(x, ParseArg(cleanType, f.Op, f.Arg), f.Op, cleanType),
                () => False);
        }

        internal static Expression BuildFilterExpression(Expression lhs, Expression rhs, Operation op, Type t)
        {
            return op switch
            {
                Operation.Eq => Equal(lhs, rhs),
                Operation.Neq => NotEqual(lhs, rhs),
                Operation.Gr => GreaterThan(lhs, rhs),
                Operation.GrEq => GreaterThanOrEqual(lhs, rhs),
                Operation.Lt => LessThan(lhs, rhs),
                Operation.LtEq => LessThanOrEqual(lhs, rhs),
                Operation.In => Call(null, ContainsMethod.MakeGenericMethod(t), rhs, lhs), // rhs.contains(lhs)
                Operation.NotIn => Not(BuildFilterExpression(lhs, rhs, Operation.In, t)),
                Operation.Contains => BuildContainsExpression(lhs, rhs, t), // lhs.contains(rhs)
                Operation.DoesNotContain => Not(BuildFilterExpression(lhs, rhs, Operation.Contains, t)),
                Operation.InRange => And(LessThanOrEqual(ArrayIndex(rhs, Constant(0)), lhs),
                                         LessThanOrEqual(lhs, ArrayIndex(rhs, Constant(1)))),
                Operation.NotInRange => Not(BuildFilterExpression(lhs, rhs, Operation.InRange, t)),
                _ => throw new ArgumentOutOfRangeException(nameof(op), "Please, use one of predefined operations"),
            };
        }

        internal static Expression BuildContainsExpression(Expression lhs, Expression rhs, Type t) =>
            t == typeof(string)
            ? Call(lhs, StringContainsMethod, rhs)
            : IsEnumerableType(t)
            ? (Expression)Call(null, ContainsMethod.MakeGenericMethod(t), lhs, rhs)
            : throw new ArgumentException("Contains operator can't be used for scalar value");

        internal static Expression ParseArg(Type type, Operation op, string arg)
        {
            return op switch
            {
                Operation.Eq
                or Operation.Neq
                or Operation.Gr
                or Operation.GrEq
                or Operation.Lt
                or Operation.LtEq
                or Operation.Contains
                or Operation.DoesNotContain => Constant(ParseDirect(type, arg)),
                Operation.In
                or Operation.NotIn
                or Operation.InRange
                or Operation.NotInRange => Constant(ParseArray(type, arg)),
                _ => throw new ArgumentOutOfRangeException(nameof(op), "Please use one of predefined operators"),
            };
        }

        internal static object? ParseDirect(Type t, string val) => JsonConvert.DeserializeObject(val, t);

        internal static object? ParseArray(Type t, string val) => JsonConvert.DeserializeObject(val, t.MakeArrayType());
    }
}