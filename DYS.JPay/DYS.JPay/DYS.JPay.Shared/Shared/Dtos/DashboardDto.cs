using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Dtos
{
    public class DashboardDto
    {
        public double? Completed { get; set; }
        public double? CompletedTotal { get; set; }
        public double? Pending { get; set; }
        public double? PendingTotal { get; set; }
        public double? Cancelled { get; set; }
        public double? CancelledTotal { get; set; }
    }
}
