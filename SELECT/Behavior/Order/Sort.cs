using System;
using System.Linq;
using System.Linq.Expressions;

namespace SELECT.Behavior.Order
{
    public static class Sort
    {
        public static IQueryable<TModel> Sorter<TModel>(this IQueryable<TModel> query, Entities.Order order)
        {
            if (order.Name is null or "")
            {
                return query;
            }

            Expression<Func<TModel, object>> body = GetExpression<TModel>(order.Name);
            query = order.IsAsc ? query.OrderBy(body) : query.OrderByDescending(body);
            return query;
        }
        private static Expression<Func<TEntity, object>> GetExpression<TEntity>(string prop)
        {
            ParameterExpression param = Expression.Parameter(typeof(TEntity), "p");
            string[] parts = prop.Split('.');

            Expression parent = parts.Aggregate<string, Expression>(param, Expression.Property);
            Expression conversion = Expression.Convert(parent, typeof(object));

            TryExpression tryExpression = Expression.TryCatch(Expression.Block(typeof(object), conversion),
                                                    Expression.Catch(typeof(object), Expression.Constant(null)));

            return Expression.Lambda<Func<TEntity, object>>(tryExpression, param);
        }
    }
}
