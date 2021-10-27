using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SELECT.Behavior.Select
{
    public static class Selector
    {
        public static IQueryable<TModel> ApplyItems<TModel>(this IQueryable<TModel> query, string items)
        {
            if (items is null or "")
            {
                return query;
            }
            Expression<Func<TModel, TModel>> body = BuildSelector<TModel, TModel>(items);
            query = query.Select(body);
            return query;
        }
        private static Expression<Func<TSource, TTarget>> BuildSelector<TSource, TTarget>(string members)
        {
            return BuildSelector<TSource, TTarget>(members.Split(',').Select(m => m.Trim()));
        }

        private static Expression<Func<TSource, TTarget>> BuildSelector<TSource, TTarget>(IEnumerable<string> members)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TSource), "e");
            Expression body = NewObject(typeof(TTarget), parameter, members.Select(m => m.Split('.')));
            return Expression.Lambda<Func<TSource, TTarget>>(body, parameter);
        }
        private static Expression NewObject(Type targetType, Expression source, IEnumerable<string[]> memberPaths, int depth = 0)
        {
            List<MemberBinding> bindings = new();
            ConstantExpression target = Expression.Constant(null, targetType);
            foreach (IGrouping<string, string[]> memberGroup in memberPaths.GroupBy(path => path[depth]))
            {
                string memberName = memberGroup.Key;
                MemberExpression targetMember = Expression.PropertyOrField(target, memberName);
                MemberExpression sourceMember = Expression.PropertyOrField(source, memberName);
                IEnumerable<string[]> childMembers = memberGroup.Where(path => depth + 1 < path.Length);
                Expression targetValue = !childMembers.Any() ? sourceMember :
                    NewObject(targetMember.Type, sourceMember, childMembers, depth + 1);
                bindings.Add(Expression.Bind(targetMember.Member, targetValue));
            }
            return Expression.MemberInit(Expression.New(targetType), bindings);
        }
    }
}
