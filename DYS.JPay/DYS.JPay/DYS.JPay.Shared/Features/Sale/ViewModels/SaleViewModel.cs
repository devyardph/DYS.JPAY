using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Extensions;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.Settings;
using DYS.JPay.Shared.Shared.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Components;
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
        private List<CategoryDto> categories = new List<CategoryDto>();
        [ObservableProperty]
        private List<ProductDto> products = new List<ProductDto>();
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
                Categories = categoryOutput.Adapt<List<CategoryDto>>();
            }
            var productOutput = await _productService.GetProductsAsync();
            if (productOutput is not null) {
                Products = productOutput.Adapt<List<ProductDto>>();
            }
            IsBusy = false;
        }
        public async Task AddOrderAsync(ProductDto product)
        {
            var count = 1;
            var id = string.Empty;
            var existingOrder = Orders?.FirstOrDefault(query => query.Product.Id == product.Id);
            if (existingOrder != null) {
                count = existingOrder.Count + 1;
                existingOrder.Count = count;
                id = existingOrder.Id.ToString();
            }
            else {
                var newId = Guid.NewGuid();
                Orders?.Add(new OrderDto { Id = newId, Product = product, Count = count });
                id = newId.ToString();
            }
            Transaction.Total = Orders?.Sum(query => query.Product.Price * query.Count);
            PendingCartId = $"cart-{id}";
        }
        public async Task ProcessPaymentAsync()
        {
            var total = Orders.Sum(query => query.Count * query.Product.Price);
            var count = Orders.Sum(query => query.Count);
            var transaction = new Transaction { 
                Date = DateTime.UtcNow, 
                CustomerName = Transaction.CustomerName, 
                PaymentMode = Transaction.PaymentMode,
                ReferenceNo =  Transaction.ReferenceNo, 
                Total = total, 
                Count = count,
                PaymentStatus = GlobalSettings.PAID,
                Status = GlobalSettings.NEW,
            };
            var items = new List<Order>();
            foreach (var item in Orders!)
            {
                items.Add(new Order
                {
                    TransactionId = transaction.Id,
                    ProductId = item.Product.Id,
                    Name = item.Product.Name,
                    Price = item.Product.Price,
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
            await _jsRuntime.InvokeVoidAsync("showProgessBar");

            Transaction = new TransactionDto();
            Orders = new List<OrderDto>();
        }      
        #endregion

        #region EVENTS
        public void OrderChanged(OrderDto order)
        {
            if (order.Count == 0) Orders?.RemoveAll(query => query.Id == order.Id);
            Transaction.Total = Orders?.Sum(query => query.Product.Price * query.Count);
        }
        public void OnDisplayChanged(string display) {
            var settings = Session.AppSettings;
            settings.Display = display;
            _sessionService.SetAppSettings(settings);
        }
        #endregion
    }
}
   