using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DYS.JPay.Shared.Features.Categories.ViewModels
{
    public partial class CategoriesViewModel : BaseViewModel
    {
        public readonly ICategoryService _categoryService;
        public CategoriesViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            SessionService sessionService,
            ICategoryService categoryService) 
            : base(navigationManager, jsRuntime, sessionService)
        {
            _categoryService = categoryService;
        }

        #region PROPERTIES
        [ObservableProperty]
        private SearchDto search = new SearchDto();

        [ObservableProperty]
        private PageDto<CategoryDto> categories = new PageDto<CategoryDto>() { Results = new List<CategoryDto>() };
        [ObservableProperty]
        private CategoryDto category = new CategoryDto();
        #endregion


        #region FUNCTIONS
        public async Task SearchCategoriesWithPagingAsync(string action = "", bool refresh = false)
        {
            IsBusy = true;
            var currentPage = refresh || Search.CurrentPage == 0 ? 1 : Search.CurrentPage;
            if (action == "next") currentPage = Search.NextEnabled ? Search.CurrentPage + 1 : Search.CurrentPage;
            else if (action == "previous") currentPage = Search.PreviousEnabled ? Search.CurrentPage - 1 : Search.CurrentPage;

            Categories = new PageDto<CategoryDto>();
            Search.CurrentPage = currentPage;
            Search.PageSize = 20;
            Search.Columns = new List<string>() { $"Name","Description","Price"};
            var output = await _categoryService.GetCategoriesAsync(Search);
            if (output is not null)
            {

                Categories = output.Adapt<PageDto<CategoryDto>>();
                Search.CurrentPage = Categories.PageIndex;

                var display = Categories!.PageIndex * Search!.PageSize;
                var show = Categories!.TotalCount >= display ? display : Categories.TotalCount;
                Search.PreviousEnabled = Categories.PageIndex > 1;
                Search.NextEnabled = Categories.PageIndex <= Categories.TotalCount && show < Categories.TotalCount;
                Search.Summary = $"showing {show} of {Categories!.TotalCount.ToString("N0")} patients";
            }
            IsBusy = false;
        }

        public async Task SubmitCategoryAsync()
        {
            IsBusy = true;
            await _categoryService.SubmitCategoryAsync(Category);
            await _jsRuntime.InvokeVoidAsync("closeOffcanvas");
            IsBusy = false;
        }

        public async Task OpenCategory(CategoryDto? category) {
            Category = category ?? new CategoryDto();
            await _jsRuntime.InvokeVoidAsync("openOffcanvas");
        }
        #endregion

    }
}
