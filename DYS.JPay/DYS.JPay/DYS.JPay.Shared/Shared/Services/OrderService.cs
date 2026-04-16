using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface IOrderService: IBaseService
    {
        Task<int> PlaceOrderAsync(Transaction order);
        Task<int> AddOrderItemsAsync(List<Order> orderItems);
        Task<PageDto<Transaction>> GetOrdersAsync(SearchDto search);
    }

    public class OrderService : BaseService,IOrderService
    {
        private readonly IRepository<Transaction> _orderRepository;
        private readonly IRepository<Order> _orderItemRepository;

        public OrderService(
            IRepository<Transaction> orderRepository,
            IRepository<Order> orderItemRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<PageDto<Transaction>> GetOrdersAsync(SearchDto search) =>
          await _orderRepository.GetPagedAsync(search.CurrentPage,
                 search.PageSize,
                 search.Keyword,
                 search.Columns);
        public async Task<int> PlaceOrderAsync(Transaction order) => await _orderRepository.InsertAsync(order);
        public async Task<int> AddOrderItemsAsync(List<Order> orderItems) => await _orderItemRepository.InsertAsync(orderItems);
    }


}
