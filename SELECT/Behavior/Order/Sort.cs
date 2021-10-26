using System;
using System.Linq;
using System.Linq.Expressions;

namespace SELECT.Behavior.Order
{
    public static class Sort
    {
        public static IQueryable<TModel> Sorter<TModel>(this IQueryable<TModel> query, Entities.Order order)
        {
            if (order.name is null || order.name =="")
            {
                return query;
            }
            var body = GetExpression<TModel>(order.name);
            if (order.IsAsc)
            {
                query = query.OrderBy(body);
            }
            else
            {
                query = query.OrderByDescending(body);
            }
            return query;
        }
        private static Expression<Func<TEntity, object>> GetExpression<TEntity>(string prop)
        {
            var param = Expression.Parameter(typeof(TEntity), "p");
            var parts = prop.Split('.');

            Expression parent = parts.Aggregate<string, Expression>(param, Expression.Property);
            Expression conversion = Expression.Convert(parent, typeof(object));

            var tryExpression = Expression.TryCatch(Expression.Block(typeof(object), conversion),
                                                    Expression.Catch(typeof(object), Expression.Constant(null)));

            return Expression.Lambda<Func<TEntity, object>>(tryExpression, param);
        }
    }
}
