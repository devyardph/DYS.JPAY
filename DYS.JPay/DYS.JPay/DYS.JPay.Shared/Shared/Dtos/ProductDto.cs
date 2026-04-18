using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Dtos
{
    public class ProductDto
    {
        public Guid? Id { get; set; } 
        public Guid? CategoryId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string Description{ get; set; }
        public string Barcode { get; set; }
        public string ImageUrl { get; set; }
        public double? Price { get; set; }
        public bool? Featured { get; set; } = false;
    }
}
