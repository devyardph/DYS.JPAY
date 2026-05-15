using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Dtos
{
    public class OrderDto : BaseEntityDto
    {
        public Guid? TransactionId { get; set; }
        public ProductDto Product { get; set; } = new ProductDto();
        public VariantDto Variant { get; set; } = new VariantDto();
        public string Title { get; set; } = string.Empty;
        public double? Price { get; set; } 
        public int Count { get; set; } = 1;
        public double Total { get; set; }
    }
}
