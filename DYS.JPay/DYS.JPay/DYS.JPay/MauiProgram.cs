using DYS.JPay.Services;
using DYS.JPay.Shared.Shared.Data;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Extensions;
using DYS.JPay.Shared.Shared.Providers;
using DYS.JPay.Shared.Shared.Repositories;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.Settings;
using DYS.JPay.Shared.Shared.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using SQLite;

namespace DYS.JPay
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Add device-specific services used by the DYS.JPay.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "jpay-7.db");
            var dbContext = new DatabaseContext(dbPath);

            Task.Run(async () => await dbContext.InitializeAsync());

            builder.Services.AddSingleton(dbContext);
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IAppSettingService, AppSettingService>();
            builder.Services.AddSingleton<SessionService>();

            builder.Services.AddTransientForViewModels(typeof(BaseViewModel).Assembly);

            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityAuthenticationStateProvider>();
            builder.Services.AddScoped<IIdentityAuthenticationStateProvider, IdentityAuthenticationStateProvider>();
            MapsterConfig.RegisterMappings();

            builder.Services.AddMauiBlazorWebView();

            //builder.ConfigureLifecycleEvents(events =>
            //{
            //#if WINDOWS
            //    events.AddWindows(windows =>
            //    {
            //        windows.OnWindowCreated(window =>
            //        {
            //            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            //            var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            //            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);

            //            // Use FullScreenPresenter instead of enum
            //            var presenter = Microsoft.UI.Windowing.FullScreenPresenter.Create();
            //            appWindow.SetPresenter(presenter);
            //        });
            //    });
            //#endif
            //});

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
