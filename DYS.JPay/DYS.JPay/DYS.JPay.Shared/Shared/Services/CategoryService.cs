using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Extensions;
using DYS.JPay.Shared.Shared.Repositories;
using DYS.JPay.Shared.Shared.Settings;
using Mapster;
namespace DYS.JPay.Shared.Shared.Services
{
    public interface ICategoryService : IBaseService
    {
        Task<List<Category>> GetCategoriesAsync();
        Task<PageDto<Category>> GetCategoriesAsync(SearchDto search);
        Task<Category> SubmitCategoryAsync(CategoryDto category);
        Task<List<Category>> SubmitCategoriesAsync(List<CategoryDto> categories);
        Task<int> DeleteCategoryAsync(Guid? id);
    }
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Logger> _loggerRepository;
        private readonly SessionService _sessionService;
        public CategoryService(
            IRepository<Category> categoryRepository,
            IRepository<Logger> loggerRepository,
            SessionService sessionService)
        {
            _categoryRepository = categoryRepository;
            _sessionService = sessionService;
            _loggerRepository = loggerRepository;

            _categoryRepository.EntityChanged += (s, e) =>
            {
                var logger = new Logger
                {
                    Type = GlobalSettings.INFO,
                    Message = $"{e.Action} entity of type {typeof(Category).Name}:  {JsonExtensions.Convert(e.Entity)}",
                    DateCreated = DateTime.UtcNow,
                    ExecutedBy = _sessionService.CurrentUser?.Name ?? string.Empty
                };
                _loggerRepository.InsertAsync(logger);
            };
        }

        public Task<List<Category>> GetCategoriesAsync() => _categoryRepository.GetAllAsync();
        public Task<PageDto<Category>> GetCategoriesAsync(SearchDto search)  =>
             _categoryRepository.GetPagedAsync(search.CurrentPage, 
                 search.PageSize, 
                 search.Keyword, 
                 search.Columns,
                 showAll: true);

        public async Task<Category> SubmitCategoryAsync(CategoryDto category)
        {
            var item = category.Adapt<Category>();
            if (category.Id == Guid.Empty ||
                category.Id == null)
            {
                item.Id = Guid.NewGuid();
                var entity = await _categoryRepository.GetAsync(x => x.Name.ToLower() == category.Name.ToLower());
                if (entity == null) {
                    await _categoryRepository.InsertAsync(item);
                    return item;
                }
                return entity ?? new Category();
            }
            else
            {
                await _categoryRepository.UpdateAsync(item);
            }
            return item;
        }

        public async Task<List<Category>> SubmitCategoriesAsync(List<CategoryDto> categories)
        {
            var items = categories.Adapt<List<Category>>();
            await _categoryRepository.InsertAsync(items);
            return items;
        }

        public async Task<int> DeleteCategoryAsync(Guid? id)
        {
            var category = await _categoryRepository.GetAsync(query => query.Id == id);
            return await _categoryRepository.DeleteAsync(category);
        }
    }

}
