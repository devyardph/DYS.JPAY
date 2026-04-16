using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Repositories;
namespace DYS.JPay.Shared.Shared.Services
{
    public interface ICategoryService : IBaseService
    {
        Task<List<Category>> GetCategoriesAsync();
        Task<PageDto<Category>> GetCategoriesAsync(SearchDto search);
    }
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryService(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public Task<List<Category>> GetCategoriesAsync() => _categoryRepository.GetAllAsync();
        public Task<PageDto<Category>> GetCategoriesAsync(SearchDto search)  =>
             _categoryRepository.GetPagedAsync(search.CurrentPage, 
                 search.PageSize, 
                 search.Keyword, 
                 search.Columns);
    }

}
