using DYS.JPay.Shared.Features.Products.Views;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
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
            await _connection.CreateTableAsync<Transaction>();
            await _connection.CreateTableAsync<Order>();
            await _connection.CreateTableAsync<Customer>();
            await _connection.CreateTableAsync<User>();
            await _connection.CreateTableAsync<AppSetting>();

            // Seed default admin
            await SeedAdminUser();
            await SeedCategories();
            await SeedProducts();
            await SeedSettings();
        }
        public SQLiteAsyncConnection Connection => _connection;

        private async Task SeedAdminUser()
        {
            var existingAdmins = await _connection.Table<User>()
                      .Where(u => u.Role.ToLower() == "admin")
                      .ToListAsync();

            if (!existingAdmins.Any())
            {
                var admin = new User
                {
                    Username = "admin",
                    Code = "123456",   // default 6-digit login code
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
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
            categories.Add(new Category
            {
                Id = new Guid("a9d3f7c2-5e1b-4c8d-9f2a-7b6e1d4f5c34"),
                Name = "Merch",
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
                Name = "Espresso",
                Price = 5.55,
                ImageUrl = "https://images.unsplash.com/photo-1607958996333-41aef7caefaa?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("c2f8a7d1-4b3e-4f9a-9a1c-8e2d6b9f3a12"),
                Type = "Pastries",
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
                Type = "Coffee",
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
                Type = "Tea",
                Name = "Espresso",
                Price = 5.55,
                ImageUrl = "https://images.unsplash.com/photo-1607958996333-41aef7caefaa?auto=format&fit=crop&q=80&w=200&h=200",
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
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Type = "Pastries",
                Name = "Latte",
                Price = 4.55,
                ImageUrl = "https://images.unsplash.com/photo-1570968915860-54d5c301fa9f?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("a9d3f7c2-5e1b-4c8d-9f2a-7b6e1d4f5c34"),
                Type = "Tea",
                Name = "Ice tea",
                Price = 2.55,
                ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("a9d3f7c2-5e1b-4c8d-9f2a-7b6e1d4f5c34"),
                Type = "Coffee",
                Name = "Turkey sandwich",
                Price = 7.55,
                ImageUrl = "https://images.unsplash.com/photo-1550507992-eb63ffee0847?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("a9d3f7c2-5e1b-4c8d-9f2a-7b6e1d4f5c34"),
                Type = "Pastries",
                Name = "Chocolate cookie",
                Price = 2.55,
                ImageUrl = "https://images.unsplash.com/photo-1499636136210-6f4ee915583e?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("a9d3f7c2-5e1b-4c8d-9f2a-7b6e1d4f5c34"),
                Type = "Tea",
                Name = "Espresso",
                Price = 5.55,
                ImageUrl = "https://images.unsplash.com/photo-1607958996333-41aef7caefaa?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            }); products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("a9d3f7c2-5e1b-4c8d-9f2a-7b6e1d4f5c34"),
                Type = "Coffee",
                Name = "Espresso",
                Price = 5.55,
                ImageUrl = "https://images.unsplash.com/photo-1607958996333-41aef7caefaa?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("a9d3f7c2-5e1b-4c8d-9f2a-7b6e1d4f5c34"),
                Type = "Pastries",
                Name = "Latte",
                Price = 4.55,
                ImageUrl = "https://images.unsplash.com/photo-1570968915860-54d5c301fa9f?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("a9d3f7c2-5e1b-4c8d-9f2a-7b6e1d4f5c34"),
                Type = "Tea",
                Name = "Ice tea",
                Price = 2.55,
                ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("a9d3f7c2-5e1b-4c8d-9f2a-7b6e1d4f5c34"),
                Type = "Coffee",
                Name = "Turkey sandwich",
                Price = 7.55,
                ImageUrl = "https://images.unsplash.com/photo-1550507992-eb63ffee0847?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("a9d3f7c2-5e1b-4c8d-9f2a-7b6e1d4f5c34"),
                Type = "Pastries",
                Name = "Chocolate cookie",
                Price = 2.55,
                ImageUrl = "https://images.unsplash.com/photo-1499636136210-6f4ee915583e?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId= new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Type = "Tea",
                Name = "Espresso",
                Price = 5.55,
                ImageUrl = "https://images.unsplash.com/photo-1607958996333-41aef7caefaa?auto=format&fit=crop&q=80&w=200&h=200",
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
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Type = "Pastries",
                Name = "Latte",
                Price = 4.55,
                ImageUrl = "https://images.unsplash.com/photo-1570968915860-54d5c301fa9f?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Type = "Tea",
                Name = "Ice tea",
                Price = 2.55,
                ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Type = "Coffee",
                Name = "Turkey sandwich",
                Price = 7.55,
                ImageUrl = "https://images.unsplash.com/photo-1550507992-eb63ffee0847?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
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
                Type = "Tea",
                Name = "Espresso",
                Price = 5.55,
                ImageUrl = "https://images.unsplash.com/photo-1607958996333-41aef7caefaa?auto=format&fit=crop&q=80&w=200&h=200",
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
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Type = "Pastries",
                Name = "Latte",
                Price = 4.55,
                ImageUrl = "https://images.unsplash.com/photo-1570968915860-54d5c301fa9f?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Type = "Tea",
                Name = "Ice tea",
                Price = 2.55,
                ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Type = "Coffee",
                Name = "Turkey sandwich",
                Price = 7.55,
                ImageUrl = "https://images.unsplash.com/photo-1550507992-eb63ffee0847?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
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
                Type = "Tea",
                Name = "Espresso",
                Price = 5.55,
                ImageUrl = "https://images.unsplash.com/photo-1607958996333-41aef7caefaa?auto=format&fit=crop&q=80&w=200&h=200",
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
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Type = "Pastries",
                Name = "Latte",
                Price = 4.55,
                ImageUrl = "https://images.unsplash.com/photo-1570968915860-54d5c301fa9f?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Type = "Tea",
                Name = "Ice tea",
                Price = 2.55,
                ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
                Type = "Coffee",
                Name = "Turkey sandwich",
                Price = 7.55,
                ImageUrl = "https://images.unsplash.com/photo-1550507992-eb63ffee0847?auto=format&fit=crop&q=80&w=200&h=200",
                Featured = false
            });
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = new Guid("7e4c9b8f-2d1a-4f6e-9c3b-1a5d8e7f4b23"),
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
                Type = "Tea",
                Name = "Espresso",
                Price = 5.55,
                ImageUrl = "https://images.unsplash.com/photo-1607958996333-41aef7caefaa?auto=format&fit=crop&q=80&w=200&h=200",
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

            if (!existingStore.Any())
            {
                var settings = new Entities.AppSetting    
                {
                   StoreName = "Lacasetta de Brit",
                   StoreDescription = "Italian restaurant",
                   Currency="PHP",
                   Display = "grid",
                   Default= true
                };

                await _connection.InsertAsync(settings);
            }
        }
    }
}
