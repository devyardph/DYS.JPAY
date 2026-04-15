using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Dtos
{
    public class OrderDto
    {
        public string CustomerName { get; set; }
        public string PaymentMode { get; set; }
        public string ReferenceNo { get; set; }
        public double? AmountTendered { get; set; }
        public double? Total { get; set; }
        public double? Count { get; set; }
        public DateTime? Date { get; set; }

    }
}
