using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Providers;
using DYS.JPay.Shared.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;


namespace DYS.JPay.Shared.Shared.ViewModels
{
    public partial class MenuViewModel : BaseViewModel
    {

        public readonly IConfiguration _configuration;
        public readonly NavigationManager _navigationManager;
        public readonly IIdentityAuthenticationStateProvider _identityAuthenticationStateProvider;

        public MenuViewModel(NavigationManager navigationManager, 
            IJSRuntime jsRuntime,
            IAppSettingService appSettingService,
            IConfiguration configuration,
            IIdentityAuthenticationStateProvider identityAuthenticationStateProvider
            ) :base(navigationManager, jsRuntime, appSettingService)
        {
            _configuration = configuration;
            _navigationManager = navigationManager;
            _identityAuthenticationStateProvider = identityAuthenticationStateProvider;
        }

        #region PROPERTIES
        [ObservableProperty]
        private List<MenuDto>  menu = new List<MenuDto>();

        [ObservableProperty]
        private string activeMenu = "";
        #endregion

        //public override Task OnInitializedAsync()
        //{
        //     _menu = new List<MenuDto>() {
        //     new MenuDto(){ Id="home", Title="Home", Icon="bx-home-smile", Path= "/", Active= false },
        //     new MenuDto(){ Id="accounts", Title="Accounts", Icon="bx-credit-card", Path= "/accounts", Active= false },
        //     new MenuDto(){ Id="report", Title="Report", Icon="bx-file", Path= "/report", Active= false },
        //    };

        //    //_activeMenu = _navigationManager.Uri.Replace(_navigationManager.BaseUri, "").Split('/').FirstOrDefault() ?? "";
        //    return base.oni();
        //}


        public async Task NavigationToPath(string id, string path)
        {
            base.NavigationToPath(path, forceLoad: false);
            ActiveMenu = id;
            await _jsRuntime.InvokeVoidAsync("toggleSidebar");
        }

        public void GoogleSign()
        {
            var googleSignInUrl = $"{_configuration["Google:Login:Url"]}";
        }

        public async Task Signout()
        {
            await _identityAuthenticationStateProvider.MarkedAsLoggedOut();
            _navigationManager.NavigateTo("/login", forceLoad: false);
        }

    }
}
