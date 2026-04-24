using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.Settings;
using DYS.JPay.Shared.Shared.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Features.Dashboard.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel
    {

        public readonly ITransactionService _transactionService;

        public DashboardViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            SessionService sessionService,
            ITransactionService transactionService) : base(navigationManager, jsRuntime, sessionService)
        {
            _transactionService = transactionService;
        }

        #region PROPERTIES
        [ObservableProperty]
        private SearchReportDto search = new SearchReportDto();
        [ObservableProperty]
        private DashboardDto dashboard = new DashboardDto();
        [ObservableProperty]
        private List<TransactionDto> transactions = new List<TransactionDto>();
        [ObservableProperty]
        private List<TransactionDto> latestTransactions = new List<TransactionDto>();
        #endregion

        #region FUNCTIONS
        public async Task SearchTransactionsAsync()
        {
            IsBusy = true;
            Transactions = new List<TransactionDto>();
            var output = await _transactionService.GetAllTransactionsAsync(query => query.DateOrdered >= Search.StartDate &&
                                                                                    query.DateOrdered <= Search.EndDate);
         
            if (output is not null)
            {
                Transactions = output.Adapt<List<TransactionDto>>();
                LatestTransactions = Transactions.OrderByDescending(query => query.DateOrdered).Take(5).ToList();
                var completed = output.Where(o => o.Status == GlobalSettings.COMPLETED).Sum(query => query.Total);
                var pending = output.Where(o => o.Status == GlobalSettings.NEW || o.Status == GlobalSettings.PREPARING).Sum(query => query.Total);
                var cancelled = output.Where(o => o.Status == GlobalSettings.CANCELLED).Sum(query => query.Total);
                Dashboard.Completed = completed;
                Dashboard.Pending = pending;
                Dashboard.Cancelled = cancelled;

                Dashboard.CompletedTotal = output.Where(o => o.Status == GlobalSettings.COMPLETED).Count();
                Dashboard.PendingTotal = output.Where(o => o.Status == GlobalSettings.NEW || o.Status == GlobalSettings.PREPARING).Count();
                Dashboard.CancelledTotal = output.Where(o => o.Status == GlobalSettings.CANCELLED).Count();
            }
            IsBusy = false;
        }
        #endregion

    }
}
