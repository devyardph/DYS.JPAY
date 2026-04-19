using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Features.Orders.ViewModels
{
    public partial class TransactionsViewModel : BaseViewModel
    {

        public readonly ITransactionService _transactionService;

        public TransactionsViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            SessionService sessionService,
            ITransactionService transactionService) : base(navigationManager, jsRuntime, sessionService)
        {
            _transactionService = transactionService;
        }

        #region PROPERTIES
        [ObservableProperty]
        private SearchDto search = new SearchDto();

        [ObservableProperty]
        private PageDto<TransactionDto> transactions = new PageDto<TransactionDto>() { Results = new List<TransactionDto>() };
        [ObservableProperty]
        private TransactionDto transaction = new TransactionDto();
        [ObservableProperty]
        private List<Order> orders = new List<Order>();
        #endregion

        #region FUNCTIONS
        public async Task SearchTransactionsWithPagingAsync(string action = "", bool refresh = false)
        {
            IsBusy = true;
            var currentPage = refresh || Search.CurrentPage == 0 ? 1 : Search.CurrentPage;
            if (action == "next") currentPage = Search.NextEnabled ? Search.CurrentPage + 1 : Search.CurrentPage;
            else if (action == "previous") currentPage = Search.PreviousEnabled ? Search.CurrentPage - 1 : Search.CurrentPage;

            Transactions = new PageDto<TransactionDto>();
            Search.CurrentPage = currentPage;
            Search.PageSize = 10;
            Search.SortColumn = "Date";

            var output = await _transactionService.GetTransactionsAsync(Search);
            if (output is not null)
            {

                Transactions = output.Adapt<PageDto<TransactionDto>>();
                Search.CurrentPage = Transactions.PageIndex;

                var display = Transactions!.PageIndex * Search!.PageSize;
                var show = Transactions!.TotalCount >= display ? display : Transactions.TotalCount;
                Search.PreviousEnabled = Transactions.PageIndex > 1;
                Search.NextEnabled = Transactions.PageIndex <= Transactions.TotalCount && show < Transactions.TotalCount;
                Search.Summary = $"showing {show} of {Transactions!.TotalCount.ToString("N0")} patients";
            }
            IsBusy = false;
        }

        public async Task OpenTransaction(TransactionDto? transaction)
        {
            Transaction = transaction ?? new TransactionDto();
            var orders = await _transactionService.GetOrderListAsync(Transaction.Id ?? Guid.Empty);
            Orders = orders;
            await _jsRuntime.InvokeVoidAsync("openOffcanvas");
        }

        public async Task UpdateTransaction(string status)
        {
            await _transactionService.UpdateTransactionAsync(Transaction?.Id ?? Guid.Empty,status, "" );
            await _jsRuntime.InvokeVoidAsync("closeOffcanvas");
            await SearchTransactionsWithPagingAsync();
        }
        #endregion

    }
}
