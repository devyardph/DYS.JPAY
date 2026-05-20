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
        private readonly IFileService _imageService;
        private readonly IAccountService _accountService;
        public OnboardingViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            IAppSettingService appSettingService,
            IUserService userService,
            IProductService productService,
            ICategoryService categoryService,
            IFileService imageService,
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
                if (string.IsNullOrEmpty(output.StoreName))
                {
                    AppSetting.StoreName = "Your store name";
                    AppSetting.StoreDescription = "Pop up mart.";
                }
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
            var coffeeId = Guid.NewGuid();
            var pastryId = Guid.NewGuid();
            var teaId = Guid.NewGuid();
            var sandwichId = Guid.NewGuid();
            var smoothieId = Guid.NewGuid();
            categories.Add(new CategoryDto
            {
                Id = coffeeId,
                Name = "Coffee",
            });
            categories.Add(new CategoryDto
            {
                Id = pastryId,
                Name = "Pastry",
            });
            categories.Add(new CategoryDto
            {
                Id = teaId,
                Name = "Tea",
            });
            categories.Add(new CategoryDto
            {
                Id = sandwichId,
                Name = "Sandwich",
            });
            categories.Add(new CategoryDto
            {
                Id = smoothieId,
                Name = "Smoothie",
            });
            await _categoryService.SubmitCategoriesAsync(categories);
            var products = new List<ProductDto>();

            products.Add(new ProductDto { Id = Guid.NewGuid(), CategoryId = coffeeId, Type = "Coffee", Name = "Cappuccino", Price = 4.25, ImageUrl = "https://images.unsplash.com/photo-1572442388796-11668a67e53d?auto=format&fit=crop&q=80&w=200&h=200", Featured = false }); // Cappuccino
            products.Add(new ProductDto { Id = Guid.NewGuid(), CategoryId = coffeeId, Type = "Coffee", Name = "Espresso", Price = 3.10, ImageUrl = "https://images.unsplash.com/photo-1512568400610-62da28bc8a13?auto=format&fit=crop&q=80&w=200&h=200", Featured = false }); // Espresso
            products.Add(new ProductDto { Id = Guid.NewGuid(), CategoryId = coffeeId, Type = "Coffee", Name = "Mocha", Price = 4.75, ImageUrl = "https://images.unsplash.com/photo-1596078841242-12f73dc697c6?auto=format&fit=crop&q=80&w=200&h=200", Featured = false }); // Mocha
            products.Add(new ProductDto { Id = Guid.NewGuid(), CategoryId = coffeeId, Type = "Coffee", Name = "Americano", Price = 3.50, ImageUrl = "https://images.unsplash.com/photo-1551030173-122aabc4489c?auto=format&fit=crop&q=80&w=200&h=200", Featured = false }); // Americano

            products.Add(new ProductDto { Id = Guid.NewGuid(), CategoryId = teaId, Type = "Tea", Name = "Green Tea", Price = 2.95, ImageUrl = "https://images.unsplash.com/photo-1627435601361-ec25f5b1d0e5?auto=format&fit=crop&q=80&w=200&h=200", Featured = false }); // Green Tea
            products.Add(new ProductDto { Id = Guid.NewGuid(), CategoryId = teaId, Type = "Tea", Name = "Black Tea", Price = 2.85, ImageUrl = "https://images.unsplash.com/photo-1617191880520-c6a69e04fa75?auto=format&fit=crop&q=80&w=200&h=200", Featured = false }); // Black Tea
            products.Add(new ProductDto { Id = Guid.NewGuid(), CategoryId = teaId, Type = "Tea", Name = "Chamomile Tea", Price = 3.15, ImageUrl = "https://images.unsplash.com/photo-1719004322339-afe59dc291d5?auto=format&fit=crop&q=80&w=200&h=200", Featured = false }); // Chamomile Tea
            products.Add(new ProductDto { Id = Guid.NewGuid(), CategoryId = teaId, Type = "Tea", Name = "Matcha Latte", Price = 4.95, ImageUrl = "https://images.unsplash.com/photo-1515823064-d6e0c04616a7?auto=format&fit=crop&q=80&w=200&h=200", Featured = false }); // Matcha Latte

            products.Add(new ProductDto { Id = Guid.NewGuid(), CategoryId = pastryId, Type = "Pastry", Name = "Croissant", Price = 2.50, ImageUrl = "https://images.unsplash.com/photo-1600521853186-93b88b3a07b0?auto=format&fit=crop&q=80&w=200&h=200", Featured = false }); // Croissant
            products.Add(new ProductDto { Id = Guid.NewGuid(), CategoryId = pastryId, Type = "Pastry", Name = "Blueberry Muffin", Price = 2.95, ImageUrl = "https://images.unsplash.com/photo-1587778306628-482a710ba9cb?auto=format&fit=crop&q=80&w=200&h=200", Featured = false }); // Blueberry Muffin
            products.Add(new ProductDto { Id = Guid.NewGuid(), CategoryId = pastryId, Type = "Pastry", Name = "Chocolate Cake", Price = 3.95, ImageUrl = "https://images.unsplash.com/photo-1606890737304-57a1ca8a5b62?auto=format&fit=crop&q=80&w=200&h=200", Featured = false }); // Chocolate Cake
            products.Add(new ProductDto { Id = Guid.NewGuid(), CategoryId = pastryId, Type = "Pastry", Name = "Donut", Price = 1.95, ImageUrl = "https://images.unsplash.com/photo-1551024601-bec78aea704b?auto=format&fit=crop&q=80&w=200&h=200", Featured = false }); // Donut

            products.Add(new ProductDto { Id = Guid.NewGuid(), CategoryId = sandwichId, Type = "Sandwich", Name = "Club Sandwich", Price = 5.95, ImageUrl = "https://images.unsplash.com/photo-1553909489-cd47e0907980?auto=format&fit=crop&q=80&w=200&h=200", Featured = false }); // Club Sandwich
            products.Add(new ProductDto { Id = Guid.NewGuid(), CategoryId = sandwichId, Type = "Sandwich", Name = "BLT Sandwich", Price = 5.25, ImageUrl = "https://images.unsplash.com/photo-1705538363245-03fe613f9eb9?auto=format&fit=crop&q=80&w=200&h=200", Featured = false }); // BLT Sandwich
            products.Add(new ProductDto { Id = Guid.NewGuid(), CategoryId = sandwichId, Type = "Sandwich", Name = "Grilled Cheese", Price = 4.75, ImageUrl = "https://images.unsplash.com/photo-1528736235302-52922df5c122?auto=format&fit=crop&q=80&w=200&h=200", Featured = false }); // Grilled Cheese

            products.Add(new ProductDto { Id = Guid.NewGuid(), CategoryId = smoothieId, Type = "Smoothie", Name = "Strawberry Smoothie", Price = 4.95, ImageUrl = "https://images.unsplash.com/photo-1621797350488-fb28c9217e3b?auto=format&fit=crop&q=80&w=200&h=200", Featured = false }); // Strawberry Smoothie
            products.Add(new ProductDto { Id = Guid.NewGuid(), CategoryId = smoothieId, Type = "Smoothie", Name = "Mango Smoothie", Price = 4.85, ImageUrl = "https://images.unsplash.com/photo-1697642452436-9c40773cbcbb?auto=format&fit=crop&q=80&w=200&h=200", Featured = false }); // Mango Smoothie
            products.Add(new ProductDto { Id = Guid.NewGuid(), CategoryId = smoothieId, Type = "Smoothie", Name = "Banana Smoothie", Price = 4.65, ImageUrl = "https://images.unsplash.com/photo-1707219811295-0f283760668b?auto=format&fit=crop&q=80&w=200&h=200", Featured = false }); // Banana Smoothie


            await _productService.SubmitProductsAsync(products);
            IsBusy = false;
        }
        #endregion

    }
}
