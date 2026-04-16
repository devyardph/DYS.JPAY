using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Entities
{
    public class Order : BaseEntity
    {
        public Guid? TransactionId { get; set; }
        public Guid? ProductId { get; set; }
        public string Name { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }
    }
}
