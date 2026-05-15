using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Entities
{
    public class Promotion : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class PromotionItem : BaseEntity
    {
        public Guid? PromotionId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public Guid? ProductId { get; set; }
        public string VariationName { get; set; } = string.Empty;
        public Guid? VariationId { get; set; }
        public double? ActualPrice { get; set; }
        public double? DiscountedPrice { get; set; }
    }
}
