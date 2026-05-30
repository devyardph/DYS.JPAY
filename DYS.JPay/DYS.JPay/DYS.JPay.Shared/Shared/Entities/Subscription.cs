using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Entities
{
    public class Subscription : BaseEntity
    {
        // Free, Basic, Growth
        public string Tier { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Order { get; set; }
        public int MaxProducts { get; set; }
        public int MaxUsers { get; set; }
        public bool AllowPromotions { get; set; }
        public bool AllowReports { get; set; }
        public bool AllowEmailNotifications { get; set; }
    }
}
