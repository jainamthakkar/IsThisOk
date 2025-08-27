namespace IsThisOk.Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> filter);
        Task<IEnumerable<T>> FindManyAsync(System.Linq.Expressions.Expression<Func<T, bool>> filter);
        Task<T> CreateAsync(T entity);
        Task<bool> UpdateAsync(string id, T entity);
        Task<bool> DeleteAsync(string id);
    }
}