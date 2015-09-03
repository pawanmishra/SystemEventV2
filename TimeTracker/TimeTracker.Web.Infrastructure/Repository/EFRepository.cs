using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Core.Framework;

namespace TimeTracker.Infrastructure.Repository
{
    public class EFRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly DbContext _dbContext;
        private readonly string _connectionString;

        public EFRepository()
        {
            // ToDo Ideally connection string should be injected via some DI container
            _connectionString = ConfigurationManager.ConnectionStrings["TimeTrackerConnectionString"].ConnectionString;

            // ToDo Tracker context should be injected via DI container
            _dbContext = new TimeTrackerContext(_connectionString);
        }

        public IQueryable<TEntity> AllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> queryable = GetAll();
            foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties)
            {
                queryable = queryable.Include<TEntity, object>(includeProperty);
            }

            return queryable;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>().AsNoTracking();
        }

        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().Where(predicate).AsNoTracking();
        }

        public virtual void AddOrUpdate(TEntity entity)
        {
            _dbContext.Set<TEntity>().AddOrUpdate(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Deleted;
            _dbContext.SaveChanges();
        }

        public void Reload(TEntity entity)
        {
            _dbContext.Entry(entity).Reload();
        }
    }
}
