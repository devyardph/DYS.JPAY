using DYS.JPay.Shared.Features.Orders.Views;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Extensions;
using DYS.JPay.Shared.Shared.Repositories;
using DYS.JPay.Shared.Shared.Settings;
using Mapster;
using System.Linq.Expressions;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface ITransactionService : IBaseService
    {
        Task<Transaction> PlaceTransactionAsync(CartDto cart);
        Task<Transaction> UpdateTransactionAsync(
              Guid transactionId,
              string status,
              string note);

        Task<List<Transaction>> GetAllTransactionsAsync(Expression<Func<Transaction, bool>> predicate);
        Task<PageDto<Transaction>> GetTransactionsAsync(SearchDto search);
        Task<List<Order>> GetOrderListAsync(Guid transactionId);
    }

    public class TransactionService : BaseService, ITransactionService
    {
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Logger> _loggerRepository;
        private readonly SessionService _sessionService;
        /// <summary>
        /// TRANSACTION SERVICE CONSTRUCTOR
        /// </summary>
        /// <param name="transactionRepository"></param>
        /// <param name="orderRepository"></param>
        public TransactionService(
            IRepository<Transaction> transactionRepository,
            IRepository<Order> orderRepository,
            IRepository<Logger> loggerRepository,
            SessionService sessionService)
        {
            _transactionRepository = transactionRepository;
            _orderRepository = orderRepository;
            _loggerRepository = loggerRepository;
            _sessionService = sessionService;

            _transactionRepository.EntityChanged += (s, e) =>
            {
                var logger = new Logger
                {
                    Type = GlobalSettings.INFO,
                    Message = $"{e.Action} entity of type {typeof(Transaction).Name}: {JsonExtensions.Convert(e.Entity)}",
                    DateCreated = DateTime.UtcNow,
                    ExecutedBy = _sessionService.CurrentUser?.Name ?? string.Empty
                };
                _loggerRepository.InsertAsync(logger);
            };
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync(Expression<Func<Transaction, bool>> predicate) =>
          await _transactionRepository.GetAllAsync(predicate);

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
            try
            {
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
            }
            catch (Exception ex)
            {
                var log = new Logger
                {
                    Id = Guid.NewGuid(),
                    Type = GlobalSettings.ERROR,
                    Message = $"Transaction {transaction.Id} error. {ex.Message}",
                    DateCreated = DateTime.UtcNow
                };
                await _loggerRepository.InsertAsync(log);
            }
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
            switch (status)
            {
                case GlobalSettings.NEW:
                    item.DateOrdered = DateTime.UtcNow;
                    break;
                case GlobalSettings.PREPARING:
                    item.DatePrepared = DateTime.UtcNow;
                    break;
                case GlobalSettings.COMPLETED:
                    item.DateCompleted = DateTime.UtcNow;
                    break;
                case GlobalSettings.CANCELLED:
                    item.DateCancelled = DateTime.UtcNow;
                    break;
            }
            item.Status = status;
            item.Note = note;
            await _transactionRepository.UpdateAsync(item);
            return item;
        }
    }


}
