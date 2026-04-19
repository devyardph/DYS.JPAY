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
    public partial class AppSettingsViewModel : BaseViewModel
    {
        private readonly IAppSettingService _appSettingService;
        public AppSettingsViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            IAppSettingService appSettingService,
            SessionService sessionService) 
            : base(navigationManager, jsRuntime, sessionService)
        {
            _appSettingService = appSettingService;
        }

        #region PROPERTIES
        [ObservableProperty]
        private AppSetting appSetting = new AppSetting();
        #endregion

        #region FUNCTIONS
        public async Task LoadAppSetting()
        {
            IsBusy = true;
            var output = await _appSettingService.GetSettingAsync();
            if (output != null) { AppSetting = output; }
            IsBusy = false;
        }
        public async Task SaveAppSettings()
        {
            IsBusy = true;
            var output = await _appSettingService.SaveChangesAsync(AppSetting);
            var settings = await _appSettingService.GetSettingAsync();
            _sessionService.SetAppSettings(settings);
            IsBusy = false;
        }

        public void SetDisplay(string display) => AppSetting.Display = display;
        #endregion

    }
}
