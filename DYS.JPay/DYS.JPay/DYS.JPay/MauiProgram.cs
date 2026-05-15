using DYS.JPay.Helpers;
using DYS.JPay.Shared.Services;
using DYS.JPay.Shared.Shared.Data;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Extensions;
using DYS.JPay.Shared.Shared.Helpers;
using DYS.JPay.Shared.Shared.Providers;
using DYS.JPay.Shared.Shared.Repositories;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.Settings;
using DYS.JPay.Shared.Shared.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using SQLite;
using System.Runtime.ConstrainedExecution;

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
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "jpay-V22.db");
            var dbContext = new DatabaseContext(dbPath);

            Task.Run(async () => await dbContext.InitializeAsync());

            builder.Services.AddSingleton(dbContext);
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddSingleton<SessionService>();

            builder.Services.AddTransientForViewModels(typeof(BaseViewModel).Assembly);
            builder.Services.AddScopedForBaseClasses(typeof(BaseService).Assembly);

            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityAuthenticationStateProvider>();
            builder.Services.AddScoped<IIdentityAuthenticationStateProvider, IdentityAuthenticationStateProvider>();

            MapsterConfig.RegisterMappings();

            var ip = NetworkHelper.GetLocalWifiIp();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri($"http://{ip}:5000") });
            builder.Services.AddScoped<IRequestProvider, RequestProvider>();
            builder.Services.AddSingleton<IImageService, ImageService>();
            

            builder.Services.AddMauiBlazorWebView();

            //PEER TO PEER
#if IOS
                builder.Services.AddSingleton<IPeerService,  DYS.JPay.Platforms.iOS.PeerService>();
                builder.Services.AddSingleton<IPhotoService, DYS.JPay.Platforms.iOS.PhotoService>();
#elif ANDROID
            builder.Services.AddSingleton<IPeerService, DYS.JPay.Platforms.Android.PeerService>();
#elif WINDOWS
                builder.Services.AddSingleton<IPeerService, DYS.JPay.Platforms.Windows.PeerService>();
                builder.Services.AddSingleton<IPhotoService, DYS.JPay.Platforms.Windows.PhotoService>();
                builder.Services.AddSingleton<IFilePickerService, DYS.JPay.Platforms.Windows.FilePickerService>();
#endif


#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            // Resolve and start
            var job = app.Services.GetService<SchedulerService>();

            return app;
        }
    }
}
