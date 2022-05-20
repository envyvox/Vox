using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Vox.Data.Extensions;

public static class EntityFrameworkExtensions
{
    public static async Task<T> CreateEntity<T>(this AppDbContext db, T entity)
    {
        var created = db.Add(entity);

        created.State = EntityState.Added;

        await db.SaveChangesAsync();

        created.State = EntityState.Detached;

        return (T) created.Entity;
    }

    public static async Task<T> CreateEntityAsync<T>(this AppDbContext db, T entity)
    {
        var created = await db.AddAsync(entity);

        created.State = EntityState.Added;

        await db.SaveChangesAsync();

        created.State = EntityState.Detached;

        return (T) created.Entity;
    }

    public static async Task<T> UpdateEntity<T>(this AppDbContext db, T entity)
    {
        var updated = db.Update(entity);

        await db.SaveChangesAsync();

        updated.State = EntityState.Detached;

        return (T) updated.Entity;
    }

    public static async Task DeleteEntity<T>(this AppDbContext db, T entity)
    {
        var deleted = db.Remove(entity);

        deleted.State = EntityState.Deleted;

        await db.SaveChangesAsync();

        deleted.State = EntityState.Detached;
    }

    /// <summary> Return ordered by random query, db must have uuid-ossp extension. </summary>
    public static IOrderedQueryable<T> OrderByRandom<T>(this DbSet<T> source) where T : class
    {
        return source.AsQueryable().OrderBy(x => Guid.NewGuid());
    }
}