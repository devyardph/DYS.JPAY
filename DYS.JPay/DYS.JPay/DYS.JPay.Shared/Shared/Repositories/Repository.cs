using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Data;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Repositories;
using SQLite;
using System.Linq.Expressions;
using DYS.JPay.Shared.Shared.Settings;


namespace DYS.JPay.Shared.Shared.Repositories;

/// <summary>
/// Generic repository implementation using SQLite-net-pcl.
/// Provides full CRUD, soft delete, pagination, and transaction support.
/// </summary>

public class Repository<T> : IRepository<T> where T : BaseEntity, new()
{
    private readonly SQLiteAsyncConnection _connection;
    public event EventHandler<RepositoryEventArgs<T>>? EntityChanged;
    public Repository(DatabaseContext context)
    {
        _connection = context.Connection;
    }

    public Task<T> GetByIdAsync(int id) => _connection.FindAsync<T>(id);
    public Task<List<T>> GetAllAsync() => _connection.Table<T>().ToListAsync();
    public Task<int> InsertAsync(T entity) {
       var item= _connection.InsertAsync(entity);
       EntityChanged?.Invoke(this, new RepositoryEventArgs<T>(entity, GlobalSettings.INSERTED));
       return item;
    }
    public Task<int> InsertAsync(List<T> entities) => _connection.InsertAllAsync(entities);
    public Task<int> UpdateAsync(T entity) {
        var item = _connection.UpdateAsync(entity);
        EntityChanged?.Invoke(this, new RepositoryEventArgs<T>(entity, GlobalSettings.UPDATED));
        return item;
    }
    public Task<int> DeleteAsync(T entity) {
        var item = _connection.DeleteAsync(entity);
        EntityChanged?.Invoke(this, new RepositoryEventArgs<T>(entity, GlobalSettings.DELETED));
        return item;
    }

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

    public async Task<PageDto<T>> GetPagedAsync(int pageIndex, 
        int pageSize, 
        string keyword = "", 
        List<string> columns= null,
        string sortColumn = null,
        bool sortDescending = false,
        bool showAll= false)
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

            if (showAll == false)
            {
               all = all.Where(query => query.IsDeleted == false).ToList();
            }

            // Apply sorting if requested
            if (!string.IsNullOrEmpty(sortColumn))
            {
                var prop = typeof(T).GetProperty(sortColumn);
                if (prop != null)
                {
                    all = sortDescending
                        ? all.OrderByDescending(x => prop.GetValue(x)).ToList()
                        : all.OrderBy(x => prop.GetValue(x)).ToList();
                }
            }


            var totalCount = all.Count;
            var index = pageIndex >0 ? pageIndex - 1 : pageIndex;
            var results = all.Skip(index).Take(pageSize).ToList();

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

            var all = await query.ToListAsync();
            // Apply sorting if requested
            if (!string.IsNullOrEmpty(sortColumn))
            {
                var prop = typeof(T).GetProperty(sortColumn);
                if (prop != null)
                {
                    all = sortDescending
                        ? all.OrderByDescending(x => prop.GetValue(x)).ToList()
                        : all.OrderBy(x => prop.GetValue(x)).ToList();
                }
            }

            if (showAll == false) all = all.Where(query => query.IsDeleted == false).ToList();
            var totalCount = all.Count();
            var results = all.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

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

public class RepositoryEventArgs<T> : EventArgs
{
    public T Entity { get; }
    public string Action { get; }

    public RepositoryEventArgs(T entity, string action)
    {
        Entity = entity;
        Action = action;
    }
}

