using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Dtos
{
    public class ProductTemplateDto 
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string Description{ get; set; }
        public string Barcode { get; set; }
        public double? Price { get; set; }
        public bool? Active { get; set; } 
    }
}
