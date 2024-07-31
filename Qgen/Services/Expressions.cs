using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using static System.Linq.Expressions.Expression;

namespace Qgen.Services
{
    public static class Expressions
    {
        public static readonly Expression True = Expression.Constant(true);
        public static readonly Expression False = Expression.Constant(false);
        public static readonly Expression Null = Expression.Constant(null);

        public static string NullableValue = nameof(Nullable<int>.Value);
        public static string NullableHasValue = nameof(Nullable<int>.HasValue);

        public static readonly MethodInfo ContainsMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(x => x.Name == $"{nameof(Enumerable.Contains)}" && x.GetParameters().Length == 2);

        public static readonly MethodInfo StringContainsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;
        public static readonly MethodInfo ObjectToStringMethod = typeof(object).GetMethod(nameof(object.ToString))!;

        public static bool IsEnumerableType(Type t) =>
            typeof(IEnumerable).IsAssignableFrom(t);

        public static bool IsNullableType(Type type) =>
            type.IsGenericType && !type.IsGenericTypeDefinition && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        public static Expression WrapInNullCheck(Expression expression, Type type, Func<Expression, Expression> ifTrue, Func<Expression> ifFalse) =>
            IsNullableType(type)
                ? Condition(Property(expression, NullableHasValue), ifTrue(Property(expression, NullableValue)), ifFalse())
                : ifTrue(expression);

        public static Type UnnullifyType(Type type) =>
            IsNullableType(type)
                ? type.GetGenericArguments()[0]
                : type;

        public static MethodInfo? GetMethod<T>(string name, Func<MethodInfo, bool> filter) => GetMethod(typeof(T), name, filter);

        public static MethodInfo? GetMethod(Type t, string name, Func<MethodInfo, bool> filter) =>
            t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
            .Where(x => x.Name == name || x.Name.StartsWith(name))
            .FirstOrDefault(filter ?? (_ => true));
    }
}
