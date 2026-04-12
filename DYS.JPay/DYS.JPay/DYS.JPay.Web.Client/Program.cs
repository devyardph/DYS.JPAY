using DYS.JPay.Shared.Shared.Data;
using DYS.JPay.Shared.Shared.Extensions;
using DYS.JPay.Shared.Shared.Providers;
using DYS.JPay.Shared.Shared.Repositories;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Web.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the DYS.JPay.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
//builder.Services.AddSingleton(new DatabaseContext("jpay.db"));
//builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
//builder.Services.AddScoped<IProductService, ProductService>();
//builder.Services.AddScoped<IOrderService, OrderService>();
//builder.Services.AddScoped<ICustomerService, CustomerService>();
//builder.Services.AddScoped<IAccountService, AccountService>();
//builder.Services.AddTransientForViewModels(typeof(BaseViewModel).Assembly);
builder.Services.AddSingleton<SessionService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityAuthenticationStateProvider>();
await builder.Build().RunAsync();
