using DYS.JPay.Common.Dtos;
using DYS.JPay.Server.Shared.Entities;
using System.Data.Common;
using System.Linq.Expressions;


namespace DYS.JPay.Server.Shared.Repositories;

/// <summary>
/// Generic repository interface for CRUD operations on any BaseEntity.
/// </summary>
public interface IRepository<T> where T : BaseEntity, new()
{
    Task<T> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
    Task<int> InsertAsync(T entity);
    Task<int> InsertAsync(List<T> entities);
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(T entity);
    Task<PageDto<T>> GetPagedAsync(int pageIndex, int pageSize);
    Task<PageDto<T>> GetPagedAsync(int pageIndex, int pageSize, string keyword = "", List<string> columns = null);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null);
    Task<T> GetAsync(Expression<Func<T, bool>> predicate = null);
}

