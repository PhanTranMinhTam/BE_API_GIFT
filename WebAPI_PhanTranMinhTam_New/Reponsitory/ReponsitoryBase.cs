using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebAPI_PhanTranMinhTam_New.Data;

namespace WebAPI_PhanTranMinhTam_New.Reponsitory
{
    public interface IRepositoryBase<T>
    {
        T Create(T entity);
        T Update(T entity);
        void Delete(T entity);
        void CreateMulti(List<T> entities);
        void DeleteMulti(List<T> entities);
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        Task<T> AddAsync(T entity);
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> expression);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
    public class ReponsitoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly MyDbContext _context;
        public ReponsitoryBase(MyDbContext context)
        {
            _context = context;
        }
        public void CreateMulti(List<T> entities) => _context.Set<T>().AddRange(entities);

        public T Create(T entity) => _context.Set<T>().Add(entity).Entity;

        public T Update(T entity) => _context.Set<T>().Update(entity).Entity;

        public IQueryable<T> FindAll() => _context.Set<T>().AsNoTracking();

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) => _context.Set<T>().Where(expression).AsNoTracking();

        public void Delete(T entity) => _context.Set<T>().Remove(entity);

        public void DeleteMulti(List<T> entities) => _context.Set<T>().RemoveRange(entities);
        public async Task<T> AddAsync(T entity)
        {
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T> result = await _context.Set<T>().AddAsync(entity);
            return result.Entity;
        }
        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().SingleOrDefaultAsync(expression);
        }
        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(expression);
        }
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AnyAsync(predicate);
        }
    }
}
