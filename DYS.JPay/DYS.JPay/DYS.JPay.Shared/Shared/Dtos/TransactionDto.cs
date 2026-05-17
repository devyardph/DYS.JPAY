using DYS.JPay.Shared.Shared.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Dtos
{
    public class TransactionDto:BaseEntityDto
    {
        public string Code { get; set; }
        public string Cashier { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string PaymentMode { get; set; } = string.Empty;
        public string ReferenceNo { get; set; } = string.Empty;
        public double? AmountTendered { get; set; }
        public double? Total { get; set; }
        public double? GrandTotal { get; set; }
        public double? SubTotal { get; set; }
        public double? Tax { get; set; }
        public double? TotalTax { get; set; }

        public double? DiscountInPercentage { get; set; } = 0;
        public double? DiscountAmount { get; set; } = 0.00;

        public double? Count { get; set; }
        public string PaymentStatus { get; set; } = GlobalSettings.PAID;
        public string Status { get; set; } = GlobalSettings.NEW;

        public string Note { get; set; }
        public bool Cancelled { get; set; } = false;
        public DateTime? DateOrdered { get; set; }
        public DateTime? DatePrepared { get; set; }
        public DateTime? DateCompleted { get; set; }
        public DateTime? DateCancelled { get; set; }

    }
}
