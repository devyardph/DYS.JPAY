using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Common.Dtos;
using DYS.JPay.Shared.Shared.Entities;
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

namespace DYS.JPay.Shared.Features.Products.ViewModels
{
    public partial class SaleViewModel : BaseViewModel
    {

        public readonly IProductService _productService;
        public readonly IOrderService _orderService;
        public readonly IServerService _serverService;
        public readonly NavigationManager _navigationManager;

        public SaleViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            IAppSettingService appSettingService,
            IProductService productService,
            IOrderService orderService,
            IServerService serverService) : base(navigationManager, jsRuntime, appSettingService)
        {
            _navigationManager = navigationManager;
            _productService = productService;
            _orderService = orderService;
            _serverService = serverService;
        }

        #region PROPERTIES
        [ObservableProperty]
        private List<ProductDto> products = new List<ProductDto>();
        [ObservableProperty]
        private List<ProductDto> filteredProducts = new List<ProductDto>();
        [ObservableProperty]
        private List<OrderItemDto> orderItems = new List<OrderItemDto>();
        [ObservableProperty]
        private OrderDto order= new OrderDto();
        [ObservableProperty]
        private string pendingCartId = string.Empty;
        [ObservableProperty]
        private SearchDto search = new SearchDto();

        #region FUNCTIONS
        public async Task GetProductsAsync()
        {
            IsBusy = true;
            var output = await _productService.GetProductsAsync();
            if (output is not null)
            {
                Products = output.Adapt<List<ProductDto>>();
                FilteredProducts = Products;
            }
            IsBusy = false;
        }
        public async Task AddOrder(ProductDto product)
        {
            var count = 1;
            var id = string.Empty;
            var existingOrder = OrderItems?.FirstOrDefault(query => query.Product.Id == product.Id);
            if (existingOrder != null)
            {
                count = existingOrder.Count + 1;
                existingOrder.Count = count;
                id = existingOrder.Id.ToString();
            }
            else
            {
                var newId = Guid.NewGuid();
                OrderItems?.Add(new OrderItemDto { Id = newId, Product = product, Count = count });
                id = newId.ToString();
            }
            Calculate();
            PendingCartId = $"cart-{id}";
        }
        public void OrderChanged(OrderItemDto order)
        {
            if(order.Count == 0)
                OrderItems?.RemoveAll(query => query.Id == order.Id);
            Calculate();
        }

        public void Calculate()
        {
            var total = OrderItems?.Sum(query => query.Product.Price * query.Count);
            Order.Total = total;
        }
        public async Task ChargeAsync() {
            var total = OrderItems.Sum(query => query.Count * query.Product.Price);
            var count = OrderItems.Sum(query => query.Count);
            var order = new Order {  Date = DateTime.UtcNow, CustomerName= Order.CustomerName, ReferenceNo= Order.ReferenceNo, Total= total, Count= count };
            await _orderService.PlaceOrderAsync(order);

            var items = new List<OrderItem>();
            var testings = new List<TestingDto>();
            foreach (var item in OrderItems!)
            {
                items.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.Product.Id,
                    Name = item.Product.Name,
                    Price = item.Product.Price,
                    Quantity = item.Count
                });
                testings.Add(new TestingDto { Title = item.Product.Name, Description = $"{item.Product.Name} @ {item.Product.Price}" });
            }

            await _serverService.SubmitTestOrders(testings);

            await _orderService.AddOrderItemsAsync(items);
            await _jsRuntime.InvokeVoidAsync("closeModal", "charge-modal");
            await _jsRuntime.InvokeVoidAsync("showProgessBar");
            Order = new OrderDto();
            OrderItems = new List<OrderItemDto>();
        }
        public void SetDisplay(string display) => AppSetting.Display = display;

        public async Task FilterProducstsAsync(string type = "")
        {
            FilteredProducts = !string.IsNullOrEmpty(type) ?
                     Products.Where(query => query.Type == type).ToList() :
                     Products.ToList();
        }
        #endregion
    }
}
    #endregion