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

        public async Task SaveSampleProductsAsync()
        {
            IsBusy = true;
            var categories = new List<CategoryDto>();
            categories.Add(new CategoryDto
            {
                Id = new Guid("c2f8a7d1-4b3e-4f9a-9a1c-8e2d6b9f3a12"),
                Name = "Coffee",
            });
            categories.Add(new CategoryDto
            {
                Id = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Name = "Pastry",
            });
            categories.Add(new CategoryDto
            {
                Id = new Guid("a9d3f7c2-5e1b-4c8d-9f2a-7b6e1d4f5c34"),
                Name = "Merch",
            });
            await _categoryService.SubmitCategoriesAsync(categories);
            var products = new List<ProductDto>();
            products.Add(new ProductDto
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("c2f8a7d1-4b3e-4f9a-9a1c-8e2d6b9f3a12"),
                Type = "Coffee",
                Name = "Espresso",
                Price = 5.55,
                ImageUrl = "https://images.unsplash.com/photo-1607958996333-41aef7caefaa?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new ProductDto
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("c2f8a7d1-4b3e-4f9a-9a1c-8e2d6b9f3a12"),
                Type = "Pastries",
                Name = "Latte",
                Price = 4.55,
                ImageUrl = "https://images.unsplash.com/photo-1570968915860-54d5c301fa9f?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new ProductDto
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("c2f8a7d1-4b3e-4f9a-9a1c-8e2d6b9f3a12"),
                Type = "Tea",
                Name = "Ice tea",
                Price = 2.55,
                ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new ProductDto
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Type = "Coffee",
                Name = "Turkey sandwich",
                Price = 7.55,
                ImageUrl = "https://images.unsplash.com/photo-1550507992-eb63ffee0847?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new ProductDto
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Type = "Pastries",
                Name = "Chocolate cookie",
                Price = 2.55,
                ImageUrl = "https://images.unsplash.com/photo-1499636136210-6f4ee915583e?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new ProductDto
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Type = "Tea",
                Name = "Espresso",
                Price = 5.55,
                ImageUrl = "https://images.unsplash.com/photo-1607958996333-41aef7caefaa?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new ProductDto
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Type = "Coffee",
                Name = "Espresso",
                Price = 5.55,
                ImageUrl = "https://images.unsplash.com/photo-1607958996333-41aef7caefaa?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new ProductDto
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Type = "Pastries",
                Name = "Latte",
                Price = 4.55,
                ImageUrl = "https://images.unsplash.com/photo-1570968915860-54d5c301fa9f?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            await _productService.SubmitProductsAsync(products);
            IsBusy = false;
        }
        #endregion

    }
}
