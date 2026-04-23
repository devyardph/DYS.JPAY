using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Repositories;
using Mapster;
namespace DYS.JPay.Shared.Shared.Services
{
    public interface ICategoryService : IBaseService
    {
        Task<List<Category>> GetCategoriesAsync();
        Task<PageDto<Category>> GetCategoriesAsync(SearchDto search);
        Task<Category> SubmitCategoryAsync(CategoryDto category);
        Task<int> DeleteCategoryAsync(Guid? id);
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
                 search.Columns, 
                 false);

        public async Task<Category> SubmitCategoryAsync(CategoryDto category)
        {
            var item = category.Adapt<Category>();
            if (category.Id == Guid.Empty ||
                category.Id == null)
            {
                item.Id = Guid.NewGuid();
                await _categoryRepository.InsertAsync(item);
            }
            else
            {
                await _categoryRepository.UpdateAsync(item);
            }
            return item;
        }

        public async Task<int> DeleteCategoryAsync(Guid? id)
        {
            var category = await _categoryRepository.GetAsync(query => query.Id == id);
            return await _categoryRepository.DeleteAsync(category);
        }
    }

}
