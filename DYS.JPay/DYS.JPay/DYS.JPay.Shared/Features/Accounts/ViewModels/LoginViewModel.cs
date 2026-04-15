using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Common.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Repositories;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DYS.JPay.Shared.Features.Accounts.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAccountService _accountService;
        private readonly SessionService _sessionService;

        #region PROPERTIES
        [ObservableProperty]
        private LoginDto profile = new LoginDto() { Username="admin", Passcode="123456" };
        #endregion
        public LoginViewModel(
          NavigationManager navigationManager,
          IJSRuntime jsRuntime,
          IAppSettingService appSettingService,
          IAccountService accountService,
          SessionService sessionService)
         : base(navigationManager, jsRuntime, appSettingService)
        {
            _accountService = accountService;
            _sessionService = sessionService;
        }

        public async Task BasicLogin()
        {
            IsBusy = true;
            await _accountService.GetAllAsync();
            var result = await _accountService.LoginAsync(profile.Username, profile.Passcode);
            if (result != null)
            {
                _sessionService.SetUser(result);
                NavigationToPath("/", forceLoad: true);
            }
            else
            {

            }
            IsBusy = false;
        }
    }
}
