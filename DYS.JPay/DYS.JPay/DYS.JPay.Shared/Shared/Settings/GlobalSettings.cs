using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Settings
{
    public class GlobalSettings
    {

        public const string AppName = "DYS.JPay";

        //TRANSACTION STATUS: NEW, PREPARING, READY, COMPLETED
        public const string NEW = "NEW";
        public const string PREPARING = "PREPARING";
        public const string READY = "READY";
        public const string COMPLETED = "COMPLETED";

        //PAYMENT STATUS: REFUNDED, PAID,CANCELLED
        public const string PAID = "PAID";
        public const string REFUNDED = "REFUNDED";
        public const string CANCELLED = "CANCELLED";

        public const string ADMIN = "ADMIN";
        public const string CASHIER = "CASHIER";
        public const string GUEST = "GUEST";
    }
}
