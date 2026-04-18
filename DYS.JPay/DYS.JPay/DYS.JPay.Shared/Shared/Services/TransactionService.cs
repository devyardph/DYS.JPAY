using DYS.JPay.Shared.Features.Orders.Views;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Extensions;
using DYS.JPay.Shared.Shared.Repositories;
using Mapster;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface ITransactionService : IBaseService
    {
        Task<Transaction> PlaceTransactionAsync(CartDto cart);
        Task<Transaction> UpdateTransactionAsync(
              Guid transactionId,
              string status,
              string note);
        Task<PageDto<Transaction>> GetTransactionsAsync(SearchDto search);
        Task<List<Order>> GetOrderListAsync(Guid transactionId);
    }

    public class TransactionService : BaseService, ITransactionService
    {
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IRepository<Order> _orderRepository;

        /// <summary>
        /// TRANSACTION SERVICE CONSTRUCTOR
        /// </summary>
        /// <param name="transactionRepository"></param>
        /// <param name="orderRepository"></param>
        public TransactionService(
            IRepository<Transaction> transactionRepository,
            IRepository<Order> orderRepository)
        {
            _transactionRepository = transactionRepository;
            _orderRepository = orderRepository;
        }

        public async Task<PageDto<Transaction>> GetTransactionsAsync(SearchDto search) =>
          await _transactionRepository.GetPagedAsync(search.CurrentPage,
                 search.PageSize,
                 search.Keyword,
                 search.Columns,
                 search.SortColumn,
                 sortDescending:true);

        public async Task<Transaction> PlaceTransactionAsync(CartDto cart) {

            var transaction = cart.Transaction.Adapt<Transaction>();
            transaction.Id = Guid.NewGuid();
            await _transactionRepository.InsertAsync(transaction);
            var items = new List<Order>();
            foreach (var order in cart.Orders)
            {
                order.TransactionId = transaction.Id;
                var item = order.Adapt<Order>();
                item.Id = Guid.NewGuid();
                items.Add(item);
            }
            await _orderRepository.InsertAsync(items);
            return transaction;
        }

        public async Task<List<Order>> GetOrderListAsync(Guid transactionId) {
            var orders = await _orderRepository.GetAllAsync(query => query.TransactionId == transactionId);
            return orders;
        }

        public async Task<Transaction> UpdateTransactionAsync(
              Guid transactionId, 
              string status,
              string note)
        {
            var item = await _transactionRepository.GetAsync(query => query.Id == transactionId);
            item.Status = status;
            item.Note = note;
            await _transactionRepository.UpdateAsync(item);
            return item;
        }
    }


}
