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
            if (items==null || items =="")
            {
                return query;
            }
            var body = BuildSelector<TModel, TModel>(items);
            query = query.Select(body);
            return query;
        }
        private static Expression<Func<TSource, TTarget>> BuildSelector<TSource, TTarget>(string members) =>
          BuildSelector<TSource, TTarget>(members.Split(',').Select(m => m.Trim()));
        private static Expression<Func<TSource, TTarget>> BuildSelector<TSource, TTarget>(IEnumerable<string> members)
        {
            var parameter = Expression.Parameter(typeof(TSource), "e");
            var body = NewObject(typeof(TTarget), parameter, members.Select(m => m.Split('.')));
            return Expression.Lambda<Func<TSource, TTarget>>(body, parameter);
        }
        private static Expression NewObject(Type targetType, Expression source, IEnumerable<string[]> memberPaths, int depth = 0)
        {
            var bindings = new List<MemberBinding>();
            var target = Expression.Constant(null, targetType);
            foreach (var memberGroup in memberPaths.GroupBy(path => path[depth]))
            {
                var memberName = memberGroup.Key;
                var targetMember = Expression.PropertyOrField(target, memberName);
                var sourceMember = Expression.PropertyOrField(source, memberName);
                var childMembers = memberGroup.Where(path => depth + 1 < path.Length);
                var targetValue = !childMembers.Any() ? sourceMember :
                    NewObject(targetMember.Type, sourceMember, childMembers, depth + 1);
                bindings.Add(Expression.Bind(targetMember.Member, targetValue));
            }
            return Expression.MemberInit(Expression.New(targetType), bindings);
        }
    }
}
