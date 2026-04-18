using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Settings
{
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<Category, CategoryDto>.NewConfig();
            TypeAdapterConfig<CategoryDto, Category>.NewConfig();

            TypeAdapterConfig<Product, ProductDto>.NewConfig();
            TypeAdapterConfig<ProductDto, Product>.NewConfig();

            TypeAdapterConfig<Order, OrderDto>.NewConfig();
            TypeAdapterConfig<OrderDto, Order>.NewConfig();

            TypeAdapterConfig<Transaction, TransactionDto>.NewConfig();
            TypeAdapterConfig<TransactionDto, Transaction>.NewConfig();
        }
    }

}
