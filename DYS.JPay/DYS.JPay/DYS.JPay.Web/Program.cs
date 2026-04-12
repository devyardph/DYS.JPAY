using DYS.JPay.Shared.Shared.Data;
using DYS.JPay.Shared.Shared.Providers;
using DYS.JPay.Shared.Shared.Repositories;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Web.Components;
using DYS.JPay.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using DYS.JPay.Shared.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddSingleton(new DatabaseContext("jpay.db"));
//builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
//builder.Services.AddScoped<IProductService, ProductService>();
//builder.Services.AddScoped<IOrderService, OrderService>();
//builder.Services.AddScoped<ICustomerService, CustomerService>();
//builder.Services.AddScoped<IAccountService, AccountService>();
//builder.Services.AddTransientForViewModels(typeof(BaseViewModel).Assembly);
builder.Services.AddSingleton<SessionService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(options =>
   {
       options.Authority = $"";
       options.Audience = "DentalPro";

   });
builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityAuthenticationStateProvider>();

// Add device-specific services used by the DYS.JPay.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

var app = builder.Build();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(DYS.JPay.Shared._Imports).Assembly,
        typeof(DYS.JPay.Web.Client._Imports).Assembly);

app.Run();
