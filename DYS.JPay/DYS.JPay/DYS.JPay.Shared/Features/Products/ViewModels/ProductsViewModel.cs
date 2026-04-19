using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Shared.Features.Products.Components;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
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
        public readonly ICategoryService _categoryService;
        public ProductsViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            SessionService sessionService,
            IProductService patientService,
            ICategoryService categoryService) 
            : base(navigationManager, jsRuntime, sessionService)
        {
            _productService = patientService;
            _categoryService = categoryService;
        }

        #region PROPERTIES
        [ObservableProperty]
        private List<ProductDto> menu = new List<ProductDto>();
        [ObservableProperty]
        private List<CategoryDto> categories = new List<CategoryDto>();
        [ObservableProperty]
        private List<VariantDto> variants = new List<VariantDto>();
        [ObservableProperty]
        private SearchDto search = new SearchDto();

        [ObservableProperty]
        private PageDto<ProductDto> products = new PageDto<ProductDto>() { Results = new List<ProductDto>() };
        [ObservableProperty]
        private ProductDto product = new ProductDto();

        public VariantComponent VariantComponent { get; set; } = new VariantComponent();
        #endregion


        #region FUNCTIONS
        public async Task SearchProductsWithPagingAsync(string action = "", bool refresh = false)
        {
            IsBusy = true;
            var currentPage = refresh || Search.CurrentPage == 0 ? 1 : Search.CurrentPage;
            if (action == "next") currentPage = Search.NextEnabled ? Search.CurrentPage + 1 : Search.CurrentPage;
            else if (action == "previous") currentPage = Search.PreviousEnabled ? Search.CurrentPage - 1 : Search.CurrentPage;

            Products = new PageDto<ProductDto>();
            Search.CurrentPage = currentPage;
            Search.PageSize = 20;
            Search.Columns = new List<string>() { $"Name","Description","Price"};
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
            if (!Categories.Any())
            {
                var categories = await _categoryService.GetCategoriesAsync();
                if (categories is not null) Categories = categories.Adapt<List<CategoryDto>>();
            }
            IsBusy = false;
        }

        public async Task SubmitProductAsync()
        {
            IsBusy = true;
            await _productService.SubmitProductAsync(Product);
            await _jsRuntime.InvokeVoidAsync("closeOffcanvas", "product-overlay", "product-component");
            IsBusy = false;
        }
        public async Task SubmitVariantsAsync()
        {
            IsBusy = true;
            await _productService.SubmitProductWithVariantsAsync(Product, Variants);
            await _jsRuntime.InvokeVoidAsync("closeOffcanvas", "variant-overlay", "variant-component");
            IsBusy = false;
        }

        public async Task OpenProduct(ProductDto? product) {
            Product = product ?? new ProductDto();
            await _jsRuntime.InvokeVoidAsync("openOffcanvas","product-overlay","product-component");
        }

        public async Task OpenVariant(ProductDto? product)
        {
            var variants = await _productService.GetProductWithVariantsByIdAsync(product!.Id ?? Guid.Empty);
            Product = variants.product.Adapt<ProductDto>();
            Variants = variants.variants.Adapt<List<VariantDto>>();
            VariantComponent.InitializeContents(Product, Variants);
            await _jsRuntime.InvokeVoidAsync("openOffcanvas", "variant-overlay", "variant-component");
        }
        #endregion

    }
}
