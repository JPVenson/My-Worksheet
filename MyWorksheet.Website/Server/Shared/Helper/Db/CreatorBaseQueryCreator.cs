using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MyWorksheet.Website.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Helper.Db;

public static class CreatorBaseQueryCreator
{
    static CreatorBaseQueryCreator()
    {
    }

    public static void DoCreateDeleteOrUpdate<T>(this MyworksheetContext db, IEnumerable<T> existing,
        IEnumerable<T> newer, Func<T, object> comparer = null)
    {
        var state = CreateDeleteOrUpdate(db, existing, newer, comparer);
        foreach (var itemWithState in state)
        {
            switch (itemWithState.Value)
            {
                case true:
                    db.Add(itemWithState.Key);
                    break;
                case false:
                    db.Remove(itemWithState.Key);
                    break;
                case null:
                    db.Update(itemWithState.Key);
                    break;
            }
        }
    }

    public static Dictionary<TEntity, bool?> CreateDeleteOrUpdate<TEntity>(this MyworksheetContext db,
        IEnumerable<TEntity> existing,
        IEnumerable<TEntity> newer,
        Func<TEntity, object> comparer = null)
    {
        var stateDic = new Dictionary<TEntity, bool?>();
        var cache = typeof(TEntity);

        if (comparer == null)
        {

            var entityType = db.Model.FindEntityType(typeof(TEntity));
            var primaryKey = entityType.FindPrimaryKey();
            var equalExpressionParam = Expression.Parameter(typeof(TEntity));
            var property = Expression.Property(equalExpressionParam, primaryKey.GetName());

            comparer = Expression.Lambda<Func<TEntity, object>>(property, equalExpressionParam).Compile();
        }

        var newerItems = newer.ToArray();
        var original = existing.ToArray();

        foreach (var newerItem in newerItems)
        {
            var key = comparer(newerItem);
            var originalItem = original.FirstOrDefault(f => comparer(f) == key);

            //could not find the new item in the list of old items so add
            if (originalItem == null)
            {
                stateDic.Add(newerItem, true);
            }
            else
            {
                //item was found so update them
                stateDic.Add(newerItem, null);
            }
        }

        foreach (var existingItem in original)
        {
            var key = comparer(existingItem);
            var hasExistingItem = newerItems.FirstOrDefault(f => comparer(f) == key);

            //could not find the existing item in the new items so delete
            if (hasExistingItem == null)
            {
                stateDic.Add(existingItem, false);
            }
        }

        return stateDic;
    }
}