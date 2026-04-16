using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Dtos
{
    public class OrderDto
    {
        public Guid? Id { get; set; }
        public Guid? TransactionId { get; set; }
        public ProductDto Product { get; set; } = new ProductDto();
        public int Count { get; set; } = 1;
        public double Total { get; set; }
    }
}
