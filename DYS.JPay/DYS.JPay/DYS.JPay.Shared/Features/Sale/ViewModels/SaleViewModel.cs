using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Extensions;
using DYS.JPay.Shared.Shared.Services;
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

        public readonly IProductService _productService;
        public readonly IOrderService _orderService;
        public readonly IPeerService _peerService;

        public SaleViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            IAppSettingService appSettingService,
            IProductService productService,
            IOrderService orderService,
            IPeerService peerService) : base(navigationManager, jsRuntime, appSettingService)
        {
            _navigationManager = navigationManager;
            _productService = productService;
            _orderService = orderService;
            _peerService = peerService;
        }

        #region PROPERTIES
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
        public async Task GetProductsAsync()
        {
            IsBusy = true;
            var output = await _productService.GetProductsAsync();
            if (output is not null) {
                Products = output.Adapt<List<ProductDto>>();
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
            var order = new Transaction { Date = DateTime.UtcNow, 
                   CustomerName = Transaction.CustomerName, 
                   PaymentMode = Transaction.PaymentMode,
                   ReferenceNo =  Transaction.ReferenceNo, 
                   Total = total, Count = count };
            await _orderService.PlaceOrderAsync(order);

            var items = new List<Order>();
            foreach (var item in Orders!)
            {
                items.Add(new Order
                {
                    TransactionId = order.Id,
                    ProductId = item.Product.Id,
                    Name = item.Product.Name,
                    Price = item.Product.Price,
                    Quantity = item.Count
                });
            }
            //SEND VIA PEER TO PEER
            var cart = new CartDto
            {
                Transaction = Transaction,
                Orders = Orders
            };

            _peerService.SendOrder(JsonExtensions.Convert(cart));
            await _orderService.AddOrderItemsAsync(items);

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
        public void OnDisplayChanged(string display) => AppSetting.Display = display;
        #endregion
    }
}
   