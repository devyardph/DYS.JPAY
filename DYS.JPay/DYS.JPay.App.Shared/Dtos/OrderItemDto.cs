using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Common.Dtos
{
    public class OrderItemDto
    {
        public Guid? Id { get; set; }
        public ProductDto Product { get; set; } = new ProductDto();
        public int Count { get; set; } = 1;
        public double Total { get; set; }
    }
}
