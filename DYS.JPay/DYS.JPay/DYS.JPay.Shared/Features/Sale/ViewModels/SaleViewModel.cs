using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Extensions;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.Settings;
using DYS.JPay.Shared.Shared.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Text.Json.Serialization;

namespace DYS.JPay.Shared.Features.Products.ViewModels
{
    public partial class SaleViewModel : BaseViewModel
    {
        public readonly ICategoryService _categoryService;
        public readonly IProductService _productService;
        public readonly ITransactionService _transactionService;
        public readonly IPeerService _peerService;

        public SaleViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            IAppSettingService appSettingService,
            ICategoryService categoryService,
            IProductService productService,
            ITransactionService transactionService,
            IPeerService peerService,
            SessionService sessionService) : base(navigationManager, jsRuntime, sessionService)
        {
            _categoryService = categoryService;
            _productService = productService;
            _transactionService = transactionService;
            _peerService = peerService;;
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
        #endregion

        #region FUNCTIONS
        public async Task InitiazeCategoriesAndProductsAsync()
        {
            IsBusy = true;
            var categoryOutput = await _categoryService.GetCategoriesAsync();
            if (categoryOutput is not null) {
                Categories = categoryOutput.Where(query => query.IsDeleted == false)
                            .Select(query => new SelectDto() { Id = query.Id.ToString(), Name = query.Name }).ToList();
            }
            var productOutput = await _productService.GetProductsAsync();
            if (productOutput is not null) {
                Products = productOutput.Adapt<List<ProductDto>>();
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
                Variants = items.variants.Adapt<List<VariantDto>>();
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
                        Product = product, 
                        Count = count
                    });
                    id = newId.ToString();
                }
                Transaction.Total = Orders?.Sum(query => query.Price * query.Count);
                Transaction.Tax = Session.AppSettings.Tax;
                Transaction.TotalTax = (Session.AppSettings.Tax / 100) * Transaction.Total;
                Transaction.SubTotal = Transaction.Total - Transaction.TotalTax;
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
                    Count = count });
                id = newId.ToString();
            }
            Transaction.Total = Orders?.Sum(query => query.Price * query.Count);
            Transaction.Tax = Session.AppSettings.Tax;
            Transaction.TotalTax = (Session.AppSettings.Tax / 100) * Transaction.Total;
            Transaction.SubTotal = Transaction.Total - Transaction.TotalTax;
            PendingCartId = $"cart-{id}";
            await _jsRuntime.InvokeVoidAsync("closeModal", "variants-modal");
        }
        public async Task ProcessPaymentAsync()
        {
            var total = Orders.Sum(query => query.Count * query.Price);
            var count = Orders.Sum(query => query.Count);
            var transaction = new Transaction { 
                DateOrdered = DateTime.UtcNow, 
                CustomerName = Transaction.CustomerName, 
                PaymentMode = Transaction.PaymentMode,
                ReferenceNo =  Transaction.ReferenceNo, 
                Total = total, 
                Count = count,
                PaymentStatus = GlobalSettings.PAID,
                Status = GlobalSettings.NEW,
                Cashier = Session.CurrentUser.Name
            };
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
                    Quantity = item.Count
                });
            }

            //SEND VIA PEER TO PEER
            var cart = new CartDto
            {
                Transaction = transaction.Adapt<TransactionDto>(),
                Orders = items
            };

            //SAVE TO CURRENT DEVICE
            await _transactionService.PlaceTransactionAsync(cart);
            //PASS TO OTHER MAIN DEVICE
            _peerService.SendOrder(JsonExtensions.Convert(cart));

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
            Transaction.Total = Orders?.Sum(query => query.Product.Price * query.Count);
            Transaction.Tax = Session.AppSettings.Tax;
            Transaction.TotalTax = (Session.AppSettings.Tax / 100) * Transaction.Total;
            Transaction.SubTotal = Transaction.Total - Transaction.TotalTax;
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
                           Products.Where(p => p.Name.Contains(key, StringComparison.OrdinalIgnoreCase) || p.Description.Contains(key, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        #endregion
    }
}
   