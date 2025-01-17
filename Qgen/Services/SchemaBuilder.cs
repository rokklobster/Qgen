﻿using Qgen.Contracts.Services;
using Qgen.Services.Internal;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;
using static Qgen.Services.Expressions;

namespace Qgen.Services
{
    public class SchemaBuilder<T>
    {
        private readonly SchemaContainer<T> result = new();

        public Schema<T> Result => result;

        public FieldBuilder<F> RegisterField<F>(Expression<Func<T, F>> prop)
        {
            var name = prop.Body is MemberExpression mx && mx.Member is PropertyInfo p
                ? p.Name
                : throw new InvalidOperationException("Only property reads are supported");
            var schema = result.schemata.GetOrAdd(name, () => new());
            schema.Type = p.PropertyType;
            return new FieldBuilder<F>(schema, name, result, p);
        }

        public SchemaBuilder<T> AddDefaultOrderingLayer<F>(Expression<Func<T, F>> prop, bool asc)
        {
            var name = prop.Body is MemberExpression mx && mx.Member is PropertyInfo p
                ? p.Name
                : throw new InvalidOperationException("Only property reads are supported");
            result.DefaultOrdering = new Contracts.Models.Ordering(name, asc);
            return this;
        }

        public class FieldBuilder<F>
        {
            private readonly FieldSchema fieldSchema;
            private readonly string name;
            private readonly SchemaContainer<T> typeSchema;
            private readonly PropertyInfo property;

            internal FieldBuilder(FieldSchema s, string name, SchemaContainer<T> result, PropertyInfo p)
            {
                this.fieldSchema = s;
                this.name = name;
                this.typeSchema = result;
                this.property = p;
            }

            public FieldBuilder<F> Disable(bool filtering = false, bool searching = false, bool sorting = false, bool grouping = false)
            {
                if (filtering) fieldSchema.Filter = null;
                if (searching) fieldSchema.Search = null;
                if (sorting) fieldSchema.Sort = null;
                if (grouping) fieldSchema.Group = null;

                return this;
            }

            public FieldBuilder<F> DisableAll() => Disable(true, true, true, true);
            public FieldBuilder<F> EnableAll() => Enable(true, true, true, true);

            public FieldBuilder<F> Enable(bool filtering = false, bool searching = false, bool sorting = false, bool grouping = false)
            {
                if (filtering) EnableFiltering();
                if (searching) EnableSearching();
                if (sorting) EnableSorting();
                if (grouping) EnableGrouping();

                return this;
            }

            public FieldBuilder<F> EnableFiltering(Func<ParameterExpression, Expression>? customAccess = null)
            {
                fieldSchema.Filter = customAccess ?? (px => Property(px, property));
                return this;
            }

            public FieldBuilder<F> EnableSorting(Func<ParameterExpression, Expression>? customAccess = null)
            {
                fieldSchema.Sort = customAccess ?? (px => Property(px, property));
                return this;
            }

            public FieldBuilder<F> EnableGrouping(Func<ParameterExpression, Expression>? customAccess = null)
            {
                fieldSchema.Group = customAccess ?? (px => Call(Property(px, property), ObjectToStringMethod));
                return this;
            }

            public FieldBuilder<F> EnableSearching(Func<ParameterExpression, Expression>? customAccess = null)
            {
                fieldSchema.Search =
                    customAccess ??
                    (property.PropertyType == typeof(string)
                    ? (px => Property(px, property))
                    : (px => Call(Property(px, property), ObjectToStringMethod)));
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="transformer">Function of signature property -> query -> search result</param>
            /// <returns></returns>
            public FieldBuilder<F> UseCustomSearch(Func<Expression, Expression, Expression> transformer)
            {
                typeSchema.customSearchTransformers[name] = transformer;
                return this;
            }
        }
    }
}
