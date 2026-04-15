using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Common.Dtos;
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
    public partial class AppSettingsViewModel : BaseViewModel
    {

        public readonly IOrderService _orderservice;
        public readonly NavigationManager _navigationManager;

        public AppSettingsViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            IAppSettingService appSettingService,
            IOrderService orderService) : base(navigationManager, jsRuntime, appSettingService)
        {
            _navigationManager = navigationManager;
            _orderservice = orderService;
        }

        #region PROPERTIES
        [ObservableProperty]
        private SearchDto search = new SearchDto();

        [ObservableProperty]
        private PageDto<OrderDto> orders = new PageDto<OrderDto>() { Results = new List<OrderDto>() };
        #endregion

        #region FUNCTIONS
        public async Task SearchOrdersWithPagingAsync(string action = "", bool refresh = false)
        {
            IsBusy = true;
            var currentPage = refresh || Search.CurrentPage == 0 ? 1 : Search.CurrentPage;
            if (action == "next") currentPage = Search.NextEnabled ? Search.CurrentPage + 1 : Search.CurrentPage;
            else if (action == "previous") currentPage = Search.PreviousEnabled ? Search.CurrentPage - 1 : Search.CurrentPage;

            Orders = new PageDto<OrderDto>();
            Search.CurrentPage = currentPage;
            Search.PageSize = 10;

            var output = await _orderservice.GetOrdersAsync(Search);
            if (output is not null)
            {

                Orders = output.Adapt<PageDto<OrderDto>>();
                Search.CurrentPage = Orders.PageIndex;

                var display = Orders!.PageIndex * Search!.PageSize;
                var show = Orders!.TotalCount >= display ? display : Orders.TotalCount;
                Search.PreviousEnabled = Orders.PageIndex > 1;
                Search.NextEnabled = Orders.PageIndex <= Orders.TotalCount && show < Orders.TotalCount;
                Search.Summary = $"showing {show} of {Orders!.TotalCount.ToString("N0")} patients";
            }
            IsBusy = false;
        }
        #endregion

    }
}
