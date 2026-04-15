using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Common.Dtos;
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
    public partial class AppSettingsViewModel : BaseViewModel
    {

        public AppSettingsViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            IAppSettingService appSettingService) 
            : base(navigationManager, jsRuntime, appSettingService)
        {
            _navigationManager = navigationManager;
            _appSettingService = appSettingService;
        }

        #region FUNCTIONS
        public async Task SaveAppSettings()
        {
            IsBusy = true;
            var output = await _appSettingService.SaveChangesAsync(AppSetting);
            IsBusy = false;
        }

        public void SetDisplay(string display) => AppSetting.Display = display;
        #endregion

    }
}
