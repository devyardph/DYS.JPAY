using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Shared.Features.Products.Components;
using DYS.JPay.Shared.Features.Promotions.Components;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Helpers;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DYS.JPay.Shared.Features.Promotions.ViewModels
{
    public partial class PromotionsViewModel : BaseViewModel
    {

        public readonly IProductService _productService;
        public readonly ICategoryService _categoryService;
        public readonly IPromotionService _promotionService;
        public readonly IImageService _imageService;
        public PromotionsViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            SessionService sessionService,
            IProductService patientService,
            ICategoryService categoryService,
            IPromotionService promotionService,
            IImageService imageService) 
            : base(navigationManager, jsRuntime, sessionService)
        {
            _productService = patientService;
            _categoryService = categoryService;
            _promotionService = promotionService;
            _imageService = imageService;
        }

        #region PROPERTIES
      
        [ObservableProperty]
        private SearchDto search = new SearchDto();

        [ObservableProperty]
        private List<PromotionItemDto> promotionItems = new List<PromotionItemDto>();
        [ObservableProperty]
        private PageDto<PromotionDto> promotions = new PageDto<PromotionDto>() { Results = new List<PromotionDto>() };
        [ObservableProperty]
        private PromotionDto promotion = new PromotionDto();


        public PromotionComponent PromotionComponent { get; set; } = new PromotionComponent();
        #endregion


        #region FUNCTIONS
        public async Task SearchPromotionsWithPagingAsync(string action = "", bool refresh = false)
        {
            IsBusy = true;
            var currentPage = refresh || Search.CurrentPage == 0 ? 1 : Search.CurrentPage;
            if (action == "next") currentPage = Search.NextEnabled ? Search.CurrentPage + 1 : Search.CurrentPage;
            else if (action == "previous") currentPage = Search.PreviousEnabled ? Search.CurrentPage - 1 : Search.CurrentPage;

            Promotions = new PageDto<PromotionDto>();
            Search.CurrentPage = currentPage;
            Search.PageSize = 20;
            Search.Columns = new List<string>() { $"Name","Description"};
            var output = await _promotionService.GetPromotionsAsync(Search);
            if (output is not null)
            {

                Promotions = output.Adapt<PageDto<PromotionDto>>();
                Search.CurrentPage = Promotions.PageIndex;

                var display = Promotions!.PageIndex * Search!.PageSize;
                var show = Promotions!.TotalCount >= display ? display : Promotions.TotalCount;
                Search.PreviousEnabled = Promotions.PageIndex > 1;
                Search.NextEnabled = Promotions.PageIndex <= Promotions.TotalCount && show < Promotions.TotalCount;
                Search.Summary = $"showing {show} of {Promotions!.TotalCount.ToString("N0")} p";
            }
            IsBusy = false;
        }
        public async Task SubmitPromotionAsync()
        {
            IsBusy = true;
            Notification = new NotificationDto();
            var output = await _promotionService.SubmitPromotionAsync(Promotion);
            if (output.Success)
                await _jsRuntime.InvokeVoidAsync("closeOffcanvas", "promotion-overlay", "promotion-component");
            else Notification = new NotificationDto() { Description = output.Message, Success = output.Success };
            IsBusy = false;
        }

        public async Task SubmitPromotionItemsAsync()
        {
            IsBusy = true;
            Notification = new NotificationDto();
            var output = await _promotionService.SubmitPromotionItemsAsync(PromotionItems);
            if (output.Success)
                await _jsRuntime.InvokeVoidAsync("closeOffcanvas", "pricing-overlay", "pricing-component");
            else Notification = new NotificationDto() { Description = output.Message, Success = output.Success };
            IsBusy = false;
        }

        public async Task OpenPromotion(PromotionDto? promotion)
        {
            Promotion = promotion ?? new PromotionDto();
            Notification = new NotificationDto();
            await _jsRuntime.InvokeVoidAsync("openOffcanvas", "promotion-overlay", "promotion-component");
        }

        public async Task OpenPricing(PromotionDto? promotion)
        {
            Promotion = promotion ?? new PromotionDto();
            Notification = new NotificationDto();
            var currentPromoItems = await _promotionService.GetPromotionItemsByPromotionIdAsync(Promotion.Id);
            if (currentPromoItems.Any()) {
                PromotionItems = currentPromoItems.Adapt<List<PromotionItemDto>>();
            }
            else {
                var products = await _promotionService.GetProductsAsync();
                var promoItems = new List<PromotionItemDto>();
                foreach (var product in products)
                {
                    promoItems.Add(new PromotionItemDto()
                    {
                        Id = Guid.NewGuid(),
                        PromotionId = Promotion.Id,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        ActualPrice = product.Price,
                        DiscountedPrice = product.Price,
                        IsDeleted = false
                    });
                }
                PromotionItems = promoItems;
            }
            await _jsRuntime.InvokeVoidAsync("openOffcanvas", "pricing-overlay", "pricing-component");
        }
        #endregion

    }
}
