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
            TypeAdapterConfig<Product, ProductDto>.NewConfig();
            TypeAdapterConfig<Transaction, TransactionDto>.NewConfig();
            TypeAdapterConfig<Order, OrderDto>.NewConfig();
        }
    }

}
