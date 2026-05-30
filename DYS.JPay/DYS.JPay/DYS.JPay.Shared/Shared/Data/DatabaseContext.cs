using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Settings;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Data
{
    public class DatabaseContext
    {
        private readonly SQLiteAsyncConnection _connection;

        public DatabaseContext(string dbPath)
        {
            _connection = new SQLiteAsyncConnection(dbPath);
        }

        public async Task InitializeAsync() {
            // Create tables safely
            await _connection.CreateTableAsync<Category>();
            await _connection.CreateTableAsync<Product>();
            await _connection.CreateTableAsync<Variant>();
            await _connection.CreateTableAsync<Transaction>();
            await _connection.CreateTableAsync<Order>();
            await _connection.CreateTableAsync<Customer>();
            await _connection.CreateTableAsync<User>();
            await _connection.CreateTableAsync<AppSetting>();
            await _connection.CreateTableAsync<Logger>();

            await _connection.CreateTableAsync<Promotion>();
            await _connection.CreateTableAsync<PromotionItem>();
            await _connection.CreateTableAsync<Subscription>();

            // Seed default admin
            await SeedOwnerUser();
            await SeedAdminUser();
            //await SeedCategories();
            //await SeedProducts();
            await SeedSubscriptions();
            await SeedSettings();
        }
        public SQLiteAsyncConnection Connection => _connection;

        private async Task SeedOwnerUser()
        {
            var existingAdmins = await _connection.Table<User>()
                      .Where(u => u.Role == GlobalSettings.OWNER)
                      .ToListAsync();

            if (!existingAdmins.Any())
            {
                var admin = new User
                {
                    Name="Owner",
                    Username = "owner",
                    Code = "",   // default 6-digit login code
                    Role = GlobalSettings.OWNER,
                    DateCreated = DateTime.UtcNow
                };

                await _connection.InsertAsync(admin);
            }
        }
        private async Task SeedAdminUser()
        {
            var existingAdmins = await _connection.Table<User>()
                      .Where(u => u.Role == GlobalSettings.ADMIN)
                      .ToListAsync();

            if (!existingAdmins.Any())
            {
                var admin = new User
                {
                    Name = "admin",
                    Username = "admin",
                    Code = "111111",   // default 6-digit login code
                    Role = GlobalSettings.ADMIN,
                    DateCreated = DateTime.UtcNow
                };

                await _connection.InsertAsync(admin);
            }
        }

        private async Task SeedCategories()
        {
            var categories = new List<Category>();
            categories.Add(new Category
            {
                Id = new Guid("c2f8a7d1-4b3e-4f9a-9a1c-8e2d6b9f3a12"),
                Name = "Coffee",
            });
            categories.Add(new Category
            {
                Id = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Name = "Pastry",
            });

            var entities = await _connection.Table<Category>().ToListAsync();
            if (!entities.Any())
            {
                await _connection.InsertAllAsync(categories);
            }
        }
        private async Task SeedProducts()
        {
            var products = new List<Product>();

            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("c2f8a7d1-4b3e-4f9a-9a1c-8e2d6b9f3a12"),
                Type = "Coffee",
                Name = "Latte",
                Price = 4.55,
                ImageUrl = "https://images.unsplash.com/photo-1570968915860-54d5c301fa9f?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("c2f8a7d1-4b3e-4f9a-9a1c-8e2d6b9f3a12"),
                Type = "Tea",
                Name = "Ice tea",
                Price = 2.55,
                ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId= new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Type = "Pastry",
                Name = "Turkey sandwich",
                Price = 7.55,
                ImageUrl = "https://images.unsplash.com/photo-1550507992-eb63ffee0847?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId= new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Type = "Pastries",
                Name = "Chocolate cookie",
                Price = 2.55,
                ImageUrl = "https://images.unsplash.com/photo-1499636136210-6f4ee915583e?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Type = "Coffee",
                Name = "Espresso",
                Price = 5.55,
                ImageUrl = "https://images.unsplash.com/photo-1607958996333-41aef7caefaa?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
          
 

            var entities = await _connection.Table<Product>().ToListAsync();

            if (!entities.Any())
            {
                await _connection.InsertAllAsync(products);
            }
        }
        private async Task SeedSettings()
        {
            var existingStore = await _connection.Table<AppSetting>()
                      .ToListAsync();
            var freePlan = await _connection.Table<Subscription>()
                      .Where(s => s.Tier == GlobalSettings.FREE)
                      .FirstOrDefaultAsync();   
            if (!existingStore.Any())
            {
                var settings = new Entities.AppSetting
                {
                    StoreName = "",
                    StoreDescription = "",
                    Currency = "$",
                    Display = "grid",
                    Default = true,
                    Tax = 12,
                    Setup = false,
                    ActivePlanId = freePlan?.Id,
                };

                await _connection.InsertAsync(settings);
            }
        }

        private async Task SeedSubscriptions()
        {
            var existingSubscriptions = await _connection.Table<Subscription>()
                      .ToListAsync();

            var freePlanId= new Guid("3f9a7c2b-1e4d-4a8f-9c3f-2d5b8a6f7c1d");
            var basicPlanId = new Guid("7b2f9d4e-8c1a-4e3b-9f2d-1a9c8d7e5b2a");
            var growthPlanId = new Guid("9a1c7d2f-3b4e-4c8d-8f2a-7d9b1c2e3f4a");

            if (!existingSubscriptions.Any())
            {
                var subscriptions = new List<Subscription>
                {
                     new Subscription
                     {
                         Id = freePlanId,
                         Order=1,
                         Code = "jpay_monthly_free",
                         Tier = GlobalSettings.FREE,
                         MaxProducts = 20,
                         MaxUsers = 3,
                         AllowPromotions = false,
                         AllowReports = true,
                         AllowEmailNotifications = false
                     },
                     new Subscription
                     {
                         Id = basicPlanId,
                         Order=2,
                         Code = "jpay_monthly_basic",
                         Tier = GlobalSettings.BASIC,
                         MaxProducts = 50,
                         MaxUsers = 5,
                         AllowPromotions = true,
                         AllowReports = true,
                         AllowEmailNotifications = true
                     },
                     new Subscription
                     {
                         Id = growthPlanId,
                         Order=3,
                         Code = "jpay_monthly_growth",
                         Tier = GlobalSettings.GROWTH,
                         MaxProducts = int.MaxValue,
                         MaxUsers = int.MaxValue,
                         AllowPromotions = true,
                         AllowReports = true,
                         AllowEmailNotifications = true
                     }
                 };

                await _connection.InsertAllAsync(subscriptions);
            }
        }
    }
}
