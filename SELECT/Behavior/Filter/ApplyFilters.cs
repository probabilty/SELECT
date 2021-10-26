using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;
using SELECT.Entities;
using SELECT.Utilities;
using TypeSupport;
namespace SELECT.Behavior.Filter
{
    public static class ApplyFilters
    {
        public static IQueryable<TModel> ApplyFilter<TModel>(this IQueryable<TModel> query, IList<Entities.Filter> filters)
        {
            // if (context is null) return null;
            // var query = context.AsQueryable();
            foreach (Entities.Filter t in filters)
            {
                ConstantExpression keyExpression;
                var entityParam = Expression.Parameter(typeof(TModel), t.FieldName.Split(".")[0]);
                keyExpression = Expression.Constant(t.Value);
                var field = Property<TModel>(t, entityParam);
                string[] fieldNames = t.FieldName.Split(".");
                ExtendedType fieldtype = field.Type;
                ExtendedType inputtype = keyExpression.Type;

                if (t.Value?.GetType() == typeof(string) && fieldtype.Type.IsSubclassOf(typeof(Geometry)))
                {
                    string[] text = { (string)t.Value };
                    t.Value = GeometryUtils.Wkt2GeomColl(text);
                }

                if (t.Value?.GetType() == typeof(string[]) && fieldtype.Type.IsSubclassOf(typeof(Geometry)))
                    t.Value = GeometryUtils.Wkt2GeomColl((string[])t.Value);
                if (fieldtype.IsEnum && inputtype.Type == typeof(long))
                    keyExpression = Expression.Constant(Enum.ToObject(fieldtype, t.Value!));

                Expression body;
                Type constructedListType;
                Type listType;
                MethodInfo methodInfo;

                switch (t.Op)
                {
                    case Operator.Eq:
                        body = Expression.Equal(field, keyExpression);
                        break;
                    case Operator.Lt:
                        body = Expression.LessThan(field, keyExpression);
                        break;
                    case Operator.Gt:
                        body = Expression.GreaterThan(field,
                            keyExpression);
                        break;
                    case Operator.LtE:
                        body = Expression.LessThanOrEqual(field,
                            keyExpression);
                        break;
                    case Operator.GtE:
                        body = Expression.GreaterThanOrEqual(field,
                            keyExpression);
                        break;
                    case Operator.In:
                        if (fieldtype == typeof(string) && inputtype == typeof(string))
                        {
                            methodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        }
                        else if (inputtype.Type == typeof(string) && field.Type.IsSubclassOf(typeof(Geometry)))
                        {
                            constructedListType = typeof(GeometryCollection);
                            methodInfo = constructedListType.GetMethod("Contains", new[] { typeof(Geometry) });
                        }
                        else
                        {
                            listType = typeof(List<>);
                            constructedListType =
                                listType.MakeGenericType(field.Type);
                            methodInfo = constructedListType.GetMethod("Contains",
                                new[] { field.Type });
                            t.Value = ((JArray)t.Value!)?.ToObject(constructedListType);
                            keyExpression = Expression.Constant(t.Value);
                        }

                        body = Expression.Call(keyExpression, methodInfo!, field);
                        break;
                    case Operator.Contains:
                        if (fieldtype == typeof(string) && inputtype == typeof(string))
                        {
                            methodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                            body = Expression.Call(field, methodInfo!, keyExpression);
                        }
                        else if (inputtype.Type == typeof(string) && field.Type.IsSubclassOf(typeof(Geometry)))
                        {
                            constructedListType = typeof(GeometryCollection);
                            methodInfo = constructedListType.GetMethod("Contains", new[] { typeof(Geometry) });
                            body = Expression.Call(Expression.PropertyOrField(entityParam, t.FieldName), methodInfo!,
                                keyExpression);
                        }
                        else
                        {
                            listType = typeof(List<>);
                            constructedListType =
                                listType.MakeGenericType(field.Type);
                            t.Value = ((JArray)t.Value)?.ToObject(
                                typeof(TModel).GetProperty(t.FieldName)?.PropertyType!);
                            methodInfo = field.Type
                                .GetMethod("Contains", new[] { t.Value!.GetType() });
                            keyExpression = Expression.Constant(t.Value);
                            body = Expression.Call(field, methodInfo!, keyExpression);
                        }
                        break;
                    default:
                        continue;
                }
                var predicate = Expression.Lambda<Func<TModel, bool>>(body, entityParam);
                query = query.Where(predicate);
            }

            return query;
        }
        private static Expression Property<TModel>(Entities.Filter filter, ParameterExpression param)
        {
            string[] names = filter.FieldName.Split(".");
            var entityParam = Expression.PropertyOrField(param, names[0]);
            for (int i = 1; i < names.Length; i++)
            {
                entityParam = Expression.PropertyOrField(entityParam, names[i]);
            }
            return entityParam;
        }
    }
}
