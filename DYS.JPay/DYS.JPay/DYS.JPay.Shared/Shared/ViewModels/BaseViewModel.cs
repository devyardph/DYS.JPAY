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
        public readonly NavigationManager _navigationManager;
        public readonly SessionService _sessionService;
        public readonly IJSRuntime _jsRuntime;

        #region PROPERTIES
        [ObservableProperty]
        public NotificationDto notification = new NotificationDto();
        [ObservableProperty]
        public AppSetting appSetting = new AppSetting();

        [ObservableProperty]
        public bool isBusy = false;

        [ObservableProperty]
        public bool isReadOnly = false;

        [ObservableProperty]
        public SessionService session = new SessionService();
        #endregion

        public BaseViewModel(NavigationManager navigationManager,
                            IJSRuntime jsRuntime,
                            SessionService sessionService)
        {
            _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
            _sessionService = sessionService;
            session = _sessionService;
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
