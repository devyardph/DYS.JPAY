using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;


namespace DYS.JPay.Shared.Shared.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        public NavigationManager _navigationManager;
        public IAppSettingService _appSettingService;
        public IJSRuntime _jsRuntime;

        #region PROPERTIES
        [ObservableProperty]
        public NotificationDto notification = new NotificationDto();
        [ObservableProperty]
        public AppSetting appSetting = new AppSetting();

        [ObservableProperty]
        public bool isBusy = false;

        [ObservableProperty]
        public bool isReadOnly = false;
        #endregion

        public BaseViewModel(NavigationManager navigationManager,
                            IJSRuntime jsRuntime, 
                            IAppSettingService appSettingService)
        {
            _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
            _appSettingService = appSettingService;
        }

        public async Task LoadAppSetting()  {
            if(string.IsNullOrEmpty(AppSetting.StoreName))
                AppSetting = await _appSettingService.GetSettingAsync();
        }

        public virtual void NavigationToPath(string path, bool forceLoad)
        {
            _navigationManager.NavigateTo(path, forceLoad: forceLoad);
        }

        public async Task NavigationToLink(string path, bool forceLoad)
        {
            _navigationManager.NavigateTo(path, forceLoad: forceLoad);
        }
    }
}
