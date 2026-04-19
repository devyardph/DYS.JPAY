using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Features.Products.ViewModels
{
    public partial class OrdersViewModel : BaseViewModel
    {

        public readonly IPeerService _peerService_;

        public OrdersViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            SessionService sessionService,
            IPeerService peerService) 
            : base(navigationManager, jsRuntime, sessionService)
        {
            _peerService_ = peerService;
        }

        #region PROPERTIES
        [ObservableProperty]
        private List<CartDto> carts = new List<CartDto>();
        #endregion

        #region FUNCTIONS
        public async Task ReceiveCartsAsync(string content)
        {
            var test = content;
            //IsBusy = true;
            //var currentPage = refresh || Search.CurrentPage == 0 ? 1 : Search.CurrentPage;
            //if (action == "next") currentPage = Search.NextEnabled ? Search.CurrentPage + 1 : Search.CurrentPage;
            //else if (action == "previous") currentPage = Search.PreviousEnabled ? Search.CurrentPage - 1 : Search.CurrentPage;

            //Products = new PageDto<ProductDto>();
            //Search.CurrentPage = currentPage;
            //Search.PageSize = 10;

            //var output = await _productService.GetProductsAsync(Search);
            //if (output is not null)
            //{

            //    Products = output.Adapt<PageDto<ProductDto>>();
            //    Search.CurrentPage = Products.PageIndex;

            //    var display = Products!.PageIndex * Search!.PageSize;
            //    var show = Products!.TotalCount >= display ? display : Products.TotalCount;
            //    Search.PreviousEnabled = Products.PageIndex > 1;
            //    Search.NextEnabled = Products.PageIndex <= Products.TotalCount && show < Products.TotalCount;
            //    Search.Summary = $"showing {show} of {Products!.TotalCount.ToString("N0")} patients";
            //}
            //IsBusy = false;
        }
        #endregion

    }
}
