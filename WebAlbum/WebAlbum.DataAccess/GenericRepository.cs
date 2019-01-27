using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebAlbum.DomainServices;

namespace WebAlbum.DataAccess
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {
        private readonly DatabaseContext _db;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(DatabaseContext db)
        {
            _db = db;
            _dbSet = db.Set<T>();
        }

        public IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "",
            int? page = null,
            int? pageSize = null)
        {
            var query = FilterLogic(filter, orderBy, includeProperties, page, pageSize);
            return query.ToList();
        }

        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "",
            int? page = null,
            int? pageSize = null)
        {
            var query = FilterLogic(filter, orderBy, includeProperties, page, pageSize);
            return await query.ToListAsync();
        }

        private IQueryable<T> FilterLogic(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, string includeProperties, int? page, int? pageSize)
        {
            var query = includeProperties
                .Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate<string, IQueryable<T>>(_dbSet, (current, includeProperty) => 
                    current.Include(includeProperty));

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (page.HasValue && pageSize.HasValue)
                query = query
                    .Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            return query;
        }

        public IQueryable<T> AsQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public T Create()
        {
            var entity = _dbSet.Create<T>();
            return entity;
        }

        public T GetByKey(params object[] key)
        {
            return _dbSet.Find(key);
        }

        public async Task<T> GetByKeyAsync(params object[] key)
        {
            return await _dbSet.FindAsync(key);
        }

        public T Insert(T entity)
        {
            return _dbSet.Add(entity);
        }

        public void DeleteByKey(params object[] key)
        {
            var entityToDelete = _dbSet.Find(key);

            if (_db.Entry(entityToDelete).State == EntityState.Detached)
                _dbSet.Attach(entityToDelete);
            if (entityToDelete != null)
                _dbSet.Remove(entityToDelete);
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }

        public int Count(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            return query.Count();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            return await query.CountAsync();
        }
    }
}
