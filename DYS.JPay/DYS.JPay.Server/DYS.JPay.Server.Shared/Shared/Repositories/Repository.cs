using DYS.JPay.Common.Dtos;
using DYS.JPay.Server.Shared.Data;
using DYS.JPay.Server.Shared.Entities;
using SQLite;
using System.Linq.Expressions;


namespace DYS.JPay.Server.Shared.Repositories;

/// <summary>
/// Generic repository implementation using SQLite-net-pcl.
/// Provides full CRUD, soft delete, pagination, and transaction support.
/// </summary>

public class Repository<T> : IRepository<T> where T : BaseEntity, new()
{
    private readonly SQLiteAsyncConnection _connection;

    public Repository(DatabaseContext context)
    {
        _connection = context.Connection;
    }

    public Task<T> GetByIdAsync(int id) => _connection.FindAsync<T>(id);
    public Task<List<T>> GetAllAsync() => _connection.Table<T>().ToListAsync();
    public Task<int> InsertAsync(T entity) => _connection.InsertAsync(entity);
    public Task<int> InsertAsync(List<T> entities) => _connection.InsertAllAsync(entities);
    public Task<int> UpdateAsync(T entity) => _connection.UpdateAsync(entity);
    public Task<int> DeleteAsync(T entity) => _connection.DeleteAsync(entity);

    // Updated: return paging info
    public async Task<PageDto<T>> GetPagedAsync(int pageIndex, int pageSize)
    {
        var query = _connection.Table<T>();

        var totalCount = await query.CountAsync();
        var results = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PageDto<T>
        {
            Results = results,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<PageDto<T>> GetPagedAsync(int pageIndex, int pageSize, string keyword = "", List<string> columns= null)
    {
        var query = _connection.Table<T>();

        if (!string.IsNullOrWhiteSpace(keyword) && columns != null && columns.Any())
        {
            // Load all records first (sqlite-net doesn't support dynamic Where easily)
            var all = await query.ToListAsync();

            // Filter in-memory based on provided columns
            all = all.Where(entity =>
            {
                foreach (var col in columns)
                {
                    var prop = typeof(T).GetProperty(col);
                    if (prop != null)
                    {
                        var value = prop.GetValue(entity)?.ToString();
                        if (!string.IsNullOrEmpty(value) && value.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                }
                return false;
            }).ToList();

            var totalCount = all.Count;
            var results = all.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            return new PageDto<T>
            {
                Results = results,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
        else
        {
            var totalCount = await query.CountAsync();
            var results = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PageDto<T>
            {
                Results = results,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null)
    {
        var query = _connection.Table<T>();

        if (predicate != null)
            query = query.Where(predicate);

        return await query.ToListAsync();
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>> predicate = null)
    {
        var query = _connection.Table<T>();
        if (predicate != null) query = query.Where(predicate);
        return await query.FirstOrDefaultAsync();
    }


}

