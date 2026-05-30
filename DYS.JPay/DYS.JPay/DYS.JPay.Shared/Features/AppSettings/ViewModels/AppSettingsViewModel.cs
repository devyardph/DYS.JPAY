using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Helpers;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.Settings;
using DYS.JPay.Shared.Shared.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Features.Orders.ViewModels
{
    public partial class AppSettingsViewModel : BaseViewModel
    {
        private readonly IAppSettingService _appSettingService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IPrinterService _printerService;
        public AppSettingsViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            IAppSettingService appSettingService,
            SessionService sessionService,
            ISubscriptionService subscriptionService,
            IPrinterService printerService) 
            : base(navigationManager, jsRuntime, sessionService)
        {
            _appSettingService = appSettingService;
            _subscriptionService = subscriptionService;
            _printerService = printerService;
        }

        #region PROPERTIES
        [ObservableProperty]
        private AppSetting appSetting = new AppSetting();

        [ObservableProperty]
        private List<SelectDto> timeZones = new List<SelectDto>();

        [ObservableProperty]
        private List<SubscriptionPlanDto> subscriptionPlans = new List<SubscriptionPlanDto>();

        [ObservableProperty]
        private string activePlan;
        #endregion

        #region FUNCTIONS
        public async Task LoadAppSetting()
        {
            IsBusy = true;
            var output = await _appSettingService.GetSettingAsync();
            if (output != null) { 
                 AppSetting = output;
                 var activePlanId = output.ActivePlanId?.ToString() ?? string.Empty;
                 ActivePlan = GlobalSettings.Plans?.FirstOrDefault(query => query.Id == activePlanId)?.Name;
            }
            //timezone
            TimeZones = TimeZoneInfo.GetSystemTimeZones()
                       .Select(tz => new SelectDto {
                           Id = tz.Id.ToLower().Replace("/", "-").Replace(" ", "-"),
                           Name = tz.Id,
                           DisplayName = tz.DisplayName
                       })
                       .ToList();

            IsBusy = false;
        }
        public async Task SaveAppSettings()
        {
            IsBusy = true;
            var output = await _appSettingService.SaveChangesAsync(AppSetting);
            var settings = await _appSettingService.GetSettingAsync();
            _sessionService.SetAppSettings(settings);
            await _jsRuntime.InvokeVoidAsync("closeOffcanvas", "setting-overlay", "setting-component");
            IsBusy = false;
        }

        public void SetDisplay(string display) => AppSetting.Display = display;

        public async Task OpenSetting(AppSetting setting)
        {
            IsProcessing = true;
            AppSetting = setting ?? new AppSetting();
            await _jsRuntime.InvokeVoidAsync("openOffcanvas", "setting-overlay", "setting-component");
            IsProcessing = false;
        }

        public async Task OpenPlans(AppSetting setting)
        {
            IsProcessing = true;
            SubscriptionPlans = new List<SubscriptionPlanDto>();
            var subscriptionPlans = new List<SubscriptionPlanDto>();
            AppSetting = setting ?? new AppSetting();

            var activePlan = await _subscriptionService.GetActiveSubscriptionAsync();
            var plans = await _subscriptionService.GetSubscriptionPlansAsync();
            var productPlans = await _subscriptionService.GetAllProductPlansAsync();
            AppSetting.ActivePlanId = plans.FirstOrDefault(query => query.Code == activePlan?.ProductId)?.Id;

            foreach (var plan in plans)
            {
                var productPlan = productPlans.FirstOrDefault(query => query.Name?.ToLower() == plan?.Tier.ToLower());
                subscriptionPlans.Add(new SubscriptionPlanDto()
                {
                    Id = plan.Id,
                    Code = productPlan != null ? productPlan.ProductId : "",
                    Order = plan.Order,
                    Tier = productPlan!= null ? productPlan.Name : plan.Tier,
                    Price = productPlan != null ? productPlan.LocalizedPrice : "0.00"
                });
            }
            SubscriptionPlans = subscriptionPlans;
            await _jsRuntime.InvokeVoidAsync("openModal", "subscribe-modal");
            IsProcessing = false;
        }

        public async Task TestPrint()
        {
            var device = AppSetting.DefaultPrinter ?? string.Empty;
            var connection = await _printerService.ConnectAsync(device);
            if (connection.Success)
            {
                var data = EscPosHelper.BuildReceipt(AppSetting,$"Yey! Printer test {device}");
                await _printerService.PrintAsync(data);
                await _printerService.DisconnectAsync();
            }
        }
        #endregion

        #region SUSBCRIPTIONS
        public async Task SubscribeToPlanAsync(Guid? planId) { 
          IsProcessing = true;
          var subscription = SubscriptionPlans?.Where(query => query.Id == planId).FirstOrDefault();
          if (subscription != null) {
                await _subscriptionService.PurchaseSubscriptionAsync(subscription.Code);
                AppSetting.ActivePlanId = planId;
                await _appSettingService.SaveChangesAsync(AppSetting);
            }
          IsProcessing = false;
        }
        #endregion

    }
}
