using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Entities
{
    public class AppSetting : BaseEntity
    {
        public bool Setup { get; set; } = false;
        public Guid? ActivePlanId { get; set; }
        public string StoreName { get; set; }
        public string StoreDescription { get; set; }
        public string Currency { get; set; }
        public string Branch { get; set; }
        public string Counter { get; set; }
        public string Display { get; set; } // grid or list
        public bool Default { get; set; } = false;
        public double? Tax { get; set; }

        public double? TargetDailySales { get; set; }
        public double? TargetWeeklySales { get; set; }
        public double? TargetMonthlySales { get; set; }
        public double? TargetYearlySales { get; set; }
        public string GmailAccount { get; set; }
        public string AppPassword { get; set; }
        public bool ReceiveEmailNotification { get; set; } = true;

        //Make sure to connect via bluetooth and set the printer name in the app setting
        public string DefaultPrinter { get; set; }

    }
}
