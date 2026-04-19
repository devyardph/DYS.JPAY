using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Dtos
{
    public class VariantDto 
    {
        public Guid? Id { get; set; }
        public Guid? ProductId { get; set; }
        public string Name { get; set; }
        public double? Price { get; set; }
    }
}
