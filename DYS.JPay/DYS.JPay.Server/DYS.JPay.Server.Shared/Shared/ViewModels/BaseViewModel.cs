using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Common.Dtos;
using DYS.JPay.Server.Shared.Entities;
using DYS.JPay.Shared.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;


namespace DYS.JPay.Server.Shared.Shared.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        public NavigationManager _navigationManager;
        public IJSRuntime _jsRuntime;

        #region PROPERTIES
        [ObservableProperty]
        public NotificationDto notification = new NotificationDto();
        [ObservableProperty]
        public Testing appSetting = new Testing();

        [ObservableProperty]
        public bool isBusy = false;

        [ObservableProperty]
        public bool isReadOnly = false;
        #endregion

        public BaseViewModel(NavigationManager navigationManager,
                            IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
        }

      

        public virtual void NavigationToPath(string path, bool forceLoad)
        {
            _navigationManager.NavigateTo(path, forceLoad: forceLoad);
        }

        public async Task NavigationToLink(string path, bool forceLoad)
        {
            _navigationManager.NavigateTo(path, forceLoad: forceLoad);
        }

        public async Task Trigger(string id)
        {
            await _jsRuntime.InvokeVoidAsync(id);
        }
    }
}
