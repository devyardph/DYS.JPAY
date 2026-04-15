using DYS.JPay.Common.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface IOrderService
    {
        Task<int> PlaceOrderAsync(Order order);
        Task<int> AddOrderItemsAsync(List<OrderItem> orderItems);
        Task<PageDto<Order>> GetOrdersAsync(SearchDto search);
    }

    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;

        public OrderService(
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<PageDto<Order>> GetOrdersAsync(SearchDto search) =>
          await _orderRepository.GetPagedAsync(search.CurrentPage,
                 search.PageSize,
                 search.Keyword,
                 search.Columns);
        public async Task<int> PlaceOrderAsync(Order order) => await _orderRepository.InsertAsync(order);
        public async Task<int> AddOrderItemsAsync(List<OrderItem> orderItems) => await _orderItemRepository.InsertAsync(orderItems);
    }


}
