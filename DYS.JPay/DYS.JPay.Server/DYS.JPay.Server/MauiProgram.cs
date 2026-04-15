using DYS.JPay.Server.Services;
using DYS.JPay.Server.Shared.Data;
using DYS.JPay.Server.Shared.Extensions;
using DYS.JPay.Server.Shared.Features.Dashboard.Services;
using DYS.JPay.Server.Shared.Repositories;
using DYS.JPay.Server.Shared.Services;
using DYS.JPay.Server.Shared.Shared.ViewModels;
using DYS.JPay.Shared.Shared.Services;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DYS.JPay.Server
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

            // Add device-specific services used by the DYS.JPay.Server.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            builder.Services.AddMauiBlazorWebView();

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "jpay-server.db");
            var dbContext = new DatabaseContext(dbPath);

            Task.Run(async () => await dbContext.InitializeAsync());

            builder.Services.AddSingleton(dbContext);
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            // Start local HTTP server
            builder.Services.AddSingleton<HttpListener>(sp =>
            {
                var listener = new HttpListener();
                return listener;
            });

            builder.Services.AddSingleton<ITestingService, TestingService>();
            builder.Services.AddSingleton<ISampleService, SampleService>();
            builder.Services.AddTransientForViewModels(typeof(BaseViewModel).Assembly);
            builder.Services.AddSingleton<ApplicationServer>();


#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
