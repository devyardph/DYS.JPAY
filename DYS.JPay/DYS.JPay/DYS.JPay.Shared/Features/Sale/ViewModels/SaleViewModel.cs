using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Extensions;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.Settings;
using DYS.JPay.Shared.Shared.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DYS.JPay.Shared.Features.Products.ViewModels
{
    public partial class SaleViewModel : BaseViewModel
    {
        public readonly ICategoryService _categoryService;
        public readonly IProductService _productService;
        public readonly ITransactionService _transactionService;
        public readonly IPromotionService _promotionService;
        public readonly IPeerService _peerService;

        public SaleViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            IAppSettingService appSettingService,
            ICategoryService categoryService,
            IProductService productService,
            ITransactionService transactionService,
            IPromotionService promotionService,
            //IPeerService peerService,
            SessionService sessionService) : base(navigationManager, jsRuntime, sessionService)
        {
            _categoryService = categoryService;
            _productService = productService;
            _transactionService = transactionService;
            _promotionService = promotionService;
            //_peerService = peerService;;
        }

        #region PROPERTIES
        [ObservableProperty]
        private List<SelectDto> categories = new List<SelectDto>();
        [ObservableProperty]
        private SelectDto category = new SelectDto() { Id=""};
        [ObservableProperty]
        private List<ProductDto> menuProducts = new List<ProductDto>();
        [ObservableProperty]
        private List<ProductDto> products = new List<ProductDto>();
        [ObservableProperty]
        private ProductDto product = new ProductDto();
        [ObservableProperty]
        private List<VariantDto> variants = new List<VariantDto>();
        [ObservableProperty]
        private List<OrderDto> orders = new List<OrderDto>();
        [ObservableProperty]
        private TransactionDto transaction= new TransactionDto();
        [ObservableProperty]
        private string pendingCartId = string.Empty;
        [ObservableProperty]
        private SearchDto search = new SearchDto();
        [ObservableProperty]
        private PromotionDto promotion = new PromotionDto();
        [ObservableProperty]
        private List<PromotionItemDto> promotionItems = new List<PromotionItemDto>();
        #endregion

        #region FUNCTIONS
        public async Task InitiazeCategoriesAndProductsAsync()
        {
            IsBusy = true;
            var categories = await _categoryService.GetCategoriesAsync();
            if (categories is not null) {
                Categories = categories.Where(query => query.IsDeleted == false)
                            .Select(query => new SelectDto() { Id = query.Id.ToString(), Name = query.Name }).ToList();
            }
            //GET CURRENT PROMOTION
            var now = DateTime.UtcNow;
            var startDate = now.StartOfDay();
            var endDate = now.EndOfDay();

            var promotion = await _promotionService.GetPromotionAsync(query =>
                query.StartDate <= endDate &&
                query.EndDate >= startDate);

            if (promotion != null) {
                Promotion = promotion.Adapt<PromotionDto>();
                var promotionItems = await _promotionService.GetPromotionItemsByPromotionIdAsync(Promotion.Id);
                PromotionItems = promotionItems.Adapt<List<PromotionItemDto>>();
            }

            //GET ALL PRODUCTS
            var products = await _productService.GetProductsAsync();
            if (products is not null)
            {
                Products = new List<ProductDto>();
                foreach (var product in products) {
                    var promo = PromotionItems.FirstOrDefault(query => query.ProductId == product.Id && query.VariationId == null);
                    var p = product.Adapt<ProductDto>();
                    p.DiscountedPrice = promo?.DiscountedPrice;
                    Products.Add(p);
                }
                MenuProducts = Products;
            }
            IsBusy = false;
        }
        public async Task AddOrderAsync(ProductDto product)
        {
            //CHECK IF THERE ARE VARIANTS
            var items = await _productService.GetProductWithVariantsByIdAsync(product.Id ?? Guid.Empty);
            if (items.variants.Any())
            {
                Variants = new List<VariantDto>();
                foreach (var variant in items.variants)
                {
                    var promo = PromotionItems.FirstOrDefault(query => query.ProductId == product.Id && query.VariationId == variant.Id);
                    var v = variant.Adapt<VariantDto>();
                    v.DiscountedPrice = promo?.DiscountedPrice;
                    Variants.Add(v);
                }
                Product = product;
                await _jsRuntime.InvokeVoidAsync("openModal", "variants-modal");
            }
            else
            {
                var count = 1;
                var id = string.Empty;
                var existingOrder = Orders?.FirstOrDefault(query => query.Product.Id == product.Id);
                if (existingOrder != null)
                {
                    count = existingOrder.Count + 1;
                    existingOrder.Count = count;
                    id = existingOrder.Id.ToString();
                }
                else
                {
                    var newId = Guid.NewGuid();
                    Orders?.Add(new OrderDto { 
                        Id = newId,
                        Title= product.Name,
                        Price = product.Price,
                        DiscountedPrice = product.DiscountedPrice,
                        Product = product, 
                        Count = count
                    });
                    id = newId.ToString();
                }
                TransactionChanged();
                PendingCartId = $"cart-{id}";
            }
        }
        public async Task AddOrderAsync(ProductVariantDto item)
        {
            var count = 1;
            var id = string.Empty;
            var existingOrder = Orders?.FirstOrDefault(query => query.Variant?.Id == item.Variant.Id);
            if (existingOrder != null)
            {
                count = existingOrder.Count + 1;
                existingOrder.Count = count;
                id = existingOrder.Id.ToString();
            }
            else
            {
                var newId = Guid.NewGuid();
                Orders?.Add(new OrderDto { 
                    Id = newId, 
                    Product= Product,
                    Variant= item.Variant, 
                    Title =$"{item.Product.Name}-{item.Variant.Name}",
                    Price = item.Variant.Price,
                    DiscountedPrice = item.Variant.DiscountedPrice,
                    Count = count });
                id = newId.ToString();
            }
            TransactionChanged();
            PendingCartId = $"cart-{id}";
            await _jsRuntime.InvokeVoidAsync("closeModal", "variants-modal");
        }
        public async Task ProcessPaymentAsync()
        {
            var count = Orders.Sum(query => query.Count);
            var transaction = Transaction;
            transaction.DateOrdered = DateTime.UtcNow;
            transaction.Count = count;
            transaction.PaymentStatus = GlobalSettings.PAID;
            transaction.Status = GlobalSettings.NEW;
            transaction.Cashier = Session.CurrentUser.Name;

            var items = new List<Order>();
            foreach (var item in Orders!)
            {
                items.Add(new Order
                {
                    TransactionId = transaction.Id,
                    ProductId = item.Product.Id,
                    VariantId = item.Variant.Id,
                    Name = item.Title,
                    Price = item.Price,
                    DiscountedPrice = item.DiscountedPrice,
                    Quantity = item.Count
                });
            }

            //SEND VIA PEER TO PEER
            var cart = new CartDto
            {
                Transaction = transaction,
                Orders = items
            };

            //SAVE TO CURRENT DEVICE
            await _transactionService.PlaceTransactionAsync(cart);
            //PASS TO OTHER MAIN DEVICE
            //_peerService.SendOrder(JsonExtensions.Convert(cart));

            await _jsRuntime.InvokeVoidAsync("closeModal", "charge-modal");
            await _jsRuntime.InvokeVoidAsync("openModal", "result-modal");
            Transaction = new TransactionDto();
            Orders = new List<OrderDto>();
        }      
        #endregion

        #region EVENTS
        public void OrderChanged(OrderDto order)
        {
            if (order.Count == 0) Orders?.RemoveAll(query => query.Id == order.Id);
            TransactionChanged();
        }
        public void TransactionChanged()
        {
            double? total = 0;
            foreach (var order in Orders)
            {
                if (order.DiscountedPrice > 0 &&
                    order.Price != order.DiscountedPrice)
                {
                    total += (order.DiscountedPrice * order.Count);
                }
                else {
                    total += (order.Price * order.Count);
                }
            }
            Transaction.Total = total;
            Transaction.Tax = Session.AppSettings.Tax;
            Transaction.TotalTax = (Session.AppSettings.Tax / 100) * Transaction.Total;
            Transaction.SubTotal = Transaction.Total - Transaction.TotalTax;
            var discountAmount = Transaction.Total * (Transaction.DiscountInPercentage / 100);
            Transaction.DiscountAmount = Math.Round(discountAmount ?? 0, 2);
            Transaction.GrandTotal = Transaction.Total - Transaction.DiscountAmount;
        }
        public void OnDisplayChanged(string display) {
            var settings = Session.AppSettings;
            settings.Display = display;
            _sessionService.SetAppSettings(settings);
        }
        public void SelectCategory(SelectDto category)
        {
            category.Selected = true;
            Category = category;
            MenuProducts = string.IsNullOrEmpty(category.Id) ? Products :
                           Products.Where(p => p.CategoryId == new Guid(Category.Id)).ToList();
        }
        public void SearchProducts(ChangeEventArgs e)
        {
            var key = e.Value?.ToString();
            MenuProducts = string.IsNullOrEmpty(key) ? Products :
                           Products.Where(p => 
                           (!string.IsNullOrEmpty(p.Name) &&p.Name.Contains(key, StringComparison.OrdinalIgnoreCase)) ||
                           (!string.IsNullOrEmpty(p.Code) && p.Code.Contains(key, StringComparison.OrdinalIgnoreCase)) ||
                           (!string.IsNullOrEmpty(p.Description) && p.Description.Contains(key, StringComparison.OrdinalIgnoreCase))).ToList();
        }
        #endregion
    }
}
   