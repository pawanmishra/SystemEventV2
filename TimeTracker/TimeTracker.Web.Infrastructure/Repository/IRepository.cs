using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Core.Framework;

namespace TimeTracker.Infrastructure.Repository
{
    public interface IRepository<TEntity> where TEntity : IEntity
    {
        /// <summary>
        /// Gets an IQueryable matching the specified predicate, including the specified properties
        /// </summary>
        /// <returns>A defined instance</returns>
        IQueryable<TEntity> AllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);

        /// <summary>
        /// Gets an IQueryable for all instance
        /// </summary>
        /// <returns>A defined instance</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Gets an IQueryable matching the specified predicate
        /// </summary>
        /// <returns>A defined instance</returns>
        IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Upserts the specified instance
        /// </summary>
        void AddOrUpdate(TEntity entity);

        /// <summary>
        /// Deletes the specified instance
        /// </summary>
        void Delete(TEntity entity);

        /// <summary>
        /// Reloads entity from database
        /// </summary>
        void Reload(TEntity entity);
    }
}
