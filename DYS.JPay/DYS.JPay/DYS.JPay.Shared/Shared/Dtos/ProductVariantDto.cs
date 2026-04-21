using DYS.JPay.Shared.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Dtos
{
    public class ProductVariantDto
    {
        public ProductDto Product { get; set; }
        public VariantDto Variant { get; set; } = new VariantDto();
    }
}
