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
        public readonly IIdentityAuthenticationStateProvider _identityAuthenticationStateProvider;

        public MenuViewModel(NavigationManager navigationManager, 
            IJSRuntime jsRuntime,
            SessionService sessionService,
            IConfiguration configuration,
            IIdentityAuthenticationStateProvider identityAuthenticationStateProvider
            ) :base(navigationManager, jsRuntime, sessionService)
        {
            _configuration = configuration;
            _identityAuthenticationStateProvider = identityAuthenticationStateProvider;
        }

        #region PROPERTIES
        [ObservableProperty]
        private List<MenuDto>  menu = new List<MenuDto>();

        [ObservableProperty]
        private string activeMenu = "";
        #endregion

        public void GenerateMenu()
        {
            Menu = new List<MenuDto>() {
             new MenuDto(){ Id="sale", Title="Point of Sale", Icon="bx-home-smile", Path= "/", Active= true },
             new MenuDto(){ Id="dashboard", Title="Dashboard", Icon="bx-credit-card", Path= "/dashboard", Active= false },
             new MenuDto(){ Id="products", Title="Products", Icon="bx-file", Path= "/products", Active= false },
             new MenuDto(){ Id="transactions", Title="Transactions", Icon="bx-file", Path= "/transactions", Active= false },
             new MenuDto(){ Id="reports", Title="Reports", Icon="bx-file", Path= "/reports", Active= false },
             new MenuDto(){ Id="users", Title="Users", Icon="bx-file", Path= "/transactions", Active= false },
             new MenuDto(){ Id="settings", Title="Settings", Icon="bx-file", Path= "/settings", Active= false },
            };
        }

        public async Task NavigationToPath(string id, string path)
        {
            base.NavigationToPath(path, forceLoad: false);
            ActiveMenu = id;
            await _jsRuntime.InvokeVoidAsync("toggleMainbar");
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
