using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Entities
{
    public class Variant : BaseEntity
    {
        public Guid? ProductId { get; set; }
        public string Name { get; set; }
        public double? Price { get; set; }
    }
}
