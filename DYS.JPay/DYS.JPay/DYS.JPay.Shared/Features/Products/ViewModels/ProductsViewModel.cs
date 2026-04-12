using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Features.Products.ViewModels
{
    public partial class ProductsViewModel : BaseViewModel
    {

        public readonly IProductService _productService;
        public readonly NavigationManager _navigationManager;

        public ProductsViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            IAppSettingService appSettingService,
            IProductService patientService) 
            : base(navigationManager, jsRuntime, appSettingService)
        {
            _navigationManager = navigationManager;
            _productService = patientService;
        }

        #region PROPERTIES
        [ObservableProperty]
        private List<ProductDto> menu = new List<ProductDto>();

        [ObservableProperty]
        private string activeMenu = "";

        #region PROPERTIES
        [ObservableProperty]
        private SearchDto search = new SearchDto();

        [ObservableProperty]
        private PageDto<ProductDto> products = new PageDto<ProductDto>() { Results = new List<ProductDto>() };
        #endregion
        #endregion

        public async Task NavigationToPath(string id, string path)
        {
            base.NavigationToPath(path, forceLoad: false);
            ActiveMenu = id;
            await _jsRuntime.InvokeVoidAsync("toggleSidebar");
        }

        #region FUNCTIONS
        public async Task SearchProductsWithPagingAsync(string action = "", bool refresh = false)
        {
            IsBusy = true;
            var currentPage = refresh || Search.CurrentPage == 0 ? 1 : Search.CurrentPage;
            if (action == "next") currentPage = Search.NextEnabled ? Search.CurrentPage + 1 : Search.CurrentPage;
            else if (action == "previous") currentPage = Search.PreviousEnabled ? Search.CurrentPage - 1 : Search.CurrentPage;

            Products = new PageDto<ProductDto>();
            Search.CurrentPage = currentPage;
            Search.PageSize = 10;

            var output = await _productService.GetProductsAsync(Search);
            if (output is not null)
            {

                Products = output.Adapt<PageDto<ProductDto>>();
                Search.CurrentPage = Products.PageIndex;

                var display = Products!.PageIndex * Search!.PageSize;
                var show = Products!.TotalCount >= display ? display : Products.TotalCount;
                Search.PreviousEnabled = Products.PageIndex > 1;
                Search.NextEnabled = Products.PageIndex <= Products.TotalCount && show < Products.TotalCount;
                Search.Summary = $"showing {show} of {Products!.TotalCount.ToString("N0")} patients";
            }
            IsBusy = false;
        }
        #endregion

    }
}
