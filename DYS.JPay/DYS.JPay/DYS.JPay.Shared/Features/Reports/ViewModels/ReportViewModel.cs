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
using DYS.JPay.Shared.Shared.Helpers;

namespace DYS.JPay.Shared.Features.Orders.ViewModels
{
    public partial class ReportViewModel : BaseViewModel
    {

        public readonly ITransactionService _transactionService;

        public ReportViewModel(NavigationManager navigationManager,
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
        private TransactionDto transaction = new TransactionDto();
        [ObservableProperty]
        private List<Order> orders = new List<Order>();
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
                var completed = output.Where(o => o.Status == GlobalSettings.COMPLETED).Sum(query => query.Total);
                var pending = output.Where(o => o.Status == GlobalSettings.NEW || o.Status == GlobalSettings.PREPARING).Sum(query => query.Total);
                var cancelled = output.Where(o => o.Status == GlobalSettings.CANCELLED).Sum(query => query.Total);
                Dashboard.Completed = completed;
                Dashboard.Pending = pending;
                Dashboard.Cancelled = cancelled;
            }
            IsBusy = false;
        }

        public async Task OpenReport(TransactionDto? transaction)
        {
            Transaction = transaction ?? new TransactionDto();
            var orders = await _transactionService.GetOrderListAsync(Transaction.Id ?? Guid.Empty);
            Orders = orders;
            await _jsRuntime.InvokeVoidAsync("openOffcanvas", "report-overlay", "report-component");
        }

        public async Task DownloadReport()
        {
            if (Transactions.Any())
            {
                var bytes = CsvHelpers.ExportToCsv(Transactions);
                var base64 = Convert.ToBase64String(bytes);
                await _jsRuntime.InvokeVoidAsync("downloadFile", "export.csv", "text/csv", base64);
            }
        }

        #endregion

    }
}
