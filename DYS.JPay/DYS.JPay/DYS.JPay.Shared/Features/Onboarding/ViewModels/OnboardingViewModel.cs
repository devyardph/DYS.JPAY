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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DYS.JPay.Shared.Features.Onboarding.ViewModels
{
    public partial class OnboardingViewModel : BaseViewModel
    {
        private readonly IAppSettingService _appSettingService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IImageService _imageService;
        private readonly IAccountService _accountService;
        public OnboardingViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            IAppSettingService appSettingService,
            IUserService userService,
            IProductService productService,
            ICategoryService categoryService,
            IImageService imageService,
            IAccountService accountService,
            SessionService sessionService) 
            : base(navigationManager, jsRuntime, sessionService)
        {
            _appSettingService = appSettingService;
            _userService = userService;
            _productService = productService;
            _categoryService = categoryService;
            _imageService = imageService;
            _accountService = accountService;
        }

        #region PROPERTIES
        [ObservableProperty]
        private AppSetting appSetting = new AppSetting();
        [ObservableProperty]
        private UserDto owner = new UserDto();
        [ObservableProperty]
        private ProductDto product = new ProductDto();
        [ObservableProperty]
        private CategoryDto category = new CategoryDto();
        #endregion

        #region FUNCTIONS
        public async Task LoadAppSetting()
        {
            IsBusy = true;
            var output = await _appSettingService.GetSettingAsync();
            if (output != null) { 
                AppSetting = output; 

                //CHECK IF ALREADY SETUP OR NOT
                if(AppSetting.Setup) 
                    _navigationManager.NavigateTo("/login", forceLoad: false);
            }
            IsBusy = false;
        }
        public async Task LoadInitialUser()
        {
            IsBusy = true;
            var output = await _userService.GetUserAsync(query => query.Role == GlobalSettings.OWNER);
            if (output != null) { Owner = output.Adapt<UserDto>(); }
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
        public async Task SaveOwnerDetails()
        {
            IsBusy = true;
            var output = await _userService.SubmitUserAsync(Owner);
            IsBusy = false;
        }
        public async Task SaveProductAsync()
        {
            IsBusy = true;
            var category = await _categoryService.SubmitCategoryAsync(Category);
            Product.CategoryId = category.Id;
            await _productService.SubmitProductAsync(Product);
            IsBusy = false;
        }
        public async Task SaveCategoryAsync()
        {
            IsBusy = true;
            await _categoryService.SubmitCategoryAsync(Category);
            IsBusy = false;
        }

        public async Task LoginAndNavigateToHome()
        {
            IsBusy = true;
            AppSetting.Setup = true;
            await _appSettingService.SaveChangesAsync(AppSetting);
            var result = await _accountService.LoginAsync(Owner.Username, Owner.Code);
            if (result != null)
            {
                _sessionService.SetUser(result);
                var settings = await _appSettingService.GetSettingAsync();
                _sessionService.SetAppSettings(settings);
                NavigationToPath("/", forceLoad: true);
            }
            else
            {

            }
            IsBusy = false;
        }
        #endregion

    }
}
