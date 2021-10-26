﻿using System.Linq;
using SELECT.Behavior.Filter;
using SELECT.Behavior.Order;
using SELECT.Behavior.Select;
using SELECT.Entities;

namespace SELECT
{
    public static class Select
    {
        public static IQueryable<TModel> Construct<TModel>(this IQueryable<TModel> query, Request request)
        {
            return query.ApplyFilter<TModel>(request.filters).ApplyItems<TModel>(request.Items).Sorter(request.order);
        }
    }
}
