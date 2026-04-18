using DYS.JPay.Shared.Shared.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Entities
{
    public class Transaction : BaseEntity
    {
        public DateTime? Date { get; set; }
        public string CustomerName { get; set; }
        public string PaymentMode { get; set; }
        public string ReferenceNo { get; set; }
        public double? AmountTendered { get; set; }
        public double? Total { get; set; }
        public double? Count { get; set; }

        public string PaymentStatus { get; set; } = GlobalSettings.PAID;
        public string Status { get; set; } = GlobalSettings.NEW;

        public string Note { get; set; }
    }

}
