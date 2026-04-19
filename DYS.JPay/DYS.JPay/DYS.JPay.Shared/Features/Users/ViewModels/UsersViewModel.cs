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

namespace DYS.JPay.Shared.Features.Users.ViewModels
{
    public partial class UsersViewModel : BaseViewModel
    {

        public readonly IUserService _userService;

        public UsersViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            SessionService sessionService,
            IUserService userService)
            : base(navigationManager, jsRuntime, sessionService)
        {
            _userService = userService;
        }

        #region PROPERTIES
        [ObservableProperty]
        private SearchDto search = new SearchDto();

        [ObservableProperty]
        private PageDto<UserDto> users = new PageDto<UserDto>() { Results = new List<UserDto>() };
        [ObservableProperty]
        private UserDto user = new UserDto();
        #endregion


        #region FUNCTIONS
        public async Task SearchUsersWithPagingAsync(string action = "", bool refresh = false)
        {
            IsBusy = true;
            var currentPage = refresh || Search.CurrentPage == 0 ? 1 : Search.CurrentPage;
            if (action == "next") currentPage = Search.NextEnabled ? Search.CurrentPage + 1 : Search.CurrentPage;
            else if (action == "previous") currentPage = Search.PreviousEnabled ? Search.CurrentPage - 1 : Search.CurrentPage;

            Users = new PageDto<UserDto>();
            Search.CurrentPage = currentPage;
            Search.PageSize = 20;
            Search.Columns = new List<string>() { $"Name", "Description", "Price" };
            var output = await _userService.GetUsersAsync(Search);
            if (output is not null)
            {

                Users = output.Adapt<PageDto<UserDto>>();
                Search.CurrentPage = Users.PageIndex;

                var display = Users!.PageIndex * Search!.PageSize;
                var show = Users!.TotalCount >= display ? display : Users.TotalCount;
                Search.PreviousEnabled = Users.PageIndex > 1;
                Search.NextEnabled = Users.PageIndex <= Users.TotalCount && show < Users.TotalCount;
                Search.Summary = $"showing {show} of {Users!.TotalCount.ToString("N0")} patients";
            }
            IsBusy = false;
        }

        public async Task SubmitUserAsync()
        {
            IsBusy = true;
            await _userService.SubmitUserAsync(User);
            await SearchUsersWithPagingAsync();
            await _jsRuntime.InvokeVoidAsync("closeOffcanvas");
            IsBusy = false;
        }

        public async Task OpenUser(UserDto? user)
        {
            User = user ?? new UserDto();
            await _jsRuntime.InvokeVoidAsync("openOffcanvas");
        }
        #endregion

    }
}
