using DYS.JPay.Shared.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Settings
{
    public class GlobalSettings
    {

        public const string AppName = "DYS.JPay";
        //TIER NAMES: FREE, BASIC, GROWTH
        public const string FREE = "FREE";
        public const string BASIC = "BASIC";
        public const string GROWTH = "GROWTH";

        public const string CASH = "CASH";
        public const string CREDIT = "CREDIT";

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
        public const string OWNER = "OWNER";

        public const string INFO = "INFO";
        public const string ERROR = "ERROR";

        public const string INSERTED = "INSERTED";
        public const string UPDATED = "UPDATED";
        public const string DELETED = "DELETED";

        public const string ONGOING = "ON-GOING";
        public const string EXPIRED = "EXPIRED";
        public const string UPCOMING = "UPCOMING";

        public static List<SelectDto> Currencies = new List<SelectDto>()
{
    new SelectDto() { Id="USD", Name="United States Dollar", DisplayName="$" },
    new SelectDto() { Id="EUR", Name="Euro", DisplayName="€" },
    new SelectDto() { Id="GBP", Name="British Pound", DisplayName="£" },
    new SelectDto() { Id="JPY", Name="Japanese Yen", DisplayName="¥" },
    new SelectDto() { Id="CNY", Name="Chinese Yuan", DisplayName="¥" },
    new SelectDto() { Id="INR", Name="Indian Rupee", DisplayName="₹" },
    new SelectDto() { Id="AUD", Name="Australian Dollar", DisplayName="$" },
    new SelectDto() { Id="CAD", Name="Canadian Dollar", DisplayName="$" },
    new SelectDto() { Id="CHF", Name="Swiss Franc", DisplayName="Fr" },
    new SelectDto() { Id="KRW", Name="South Korean Won", DisplayName="₩" },
    new SelectDto() { Id="PHP", Name="Philippine Peso", DisplayName="₱" },
    new SelectDto() { Id="MXN", Name="Mexican Peso", DisplayName="$" },
    new SelectDto() { Id="BRL", Name="Brazilian Real", DisplayName="R$" },
    new SelectDto() { Id="RUB", Name="Russian Ruble", DisplayName="₽" },
    new SelectDto() { Id="ZAR", Name="South African Rand", DisplayName="R" },
    new SelectDto() { Id="TRY", Name="Turkish Lira", DisplayName="₺" },
    new SelectDto() { Id="SEK", Name="Swedish Krona", DisplayName="kr" },
    new SelectDto() { Id="NOK", Name="Norwegian Krone", DisplayName="kr" },
    new SelectDto() { Id="DKK", Name="Danish Krone", DisplayName="kr" },
    new SelectDto() { Id="NZD", Name="New Zealand Dollar", DisplayName="$" },
    new SelectDto() { Id="SGD", Name="Singapore Dollar", DisplayName="$" },
    new SelectDto() { Id="HKD", Name="Hong Kong Dollar", DisplayName="$" },
    new SelectDto() { Id="AED", Name="UAE Dirham", DisplayName="د.إ" },
    new SelectDto() { Id="SAR", Name="Saudi Riyal", DisplayName="﷼" },
    new SelectDto() { Id="EGP", Name="Egyptian Pound", DisplayName="£" },
};

        public static List<SelectDto> Plans = new List<SelectDto>()
        {
            new SelectDto() { Id="3f9a7c2b-1e4d-4a8f-9c3f-2d5b8a6f7c1d", Name="Free Plan", DisplayName="Free" },
            new SelectDto() { Id="7b2f9d4e-8c1a-4e3b-9f2d-1a9c8d7e5b2a", Name="Basic Plan", DisplayName="Basic" },
            new SelectDto() { Id="9a1c7d2f-3b4e-4c8d-8f2a-7d9b1c2e3f4a", Name="Growth Plan", DisplayName="Growth" }
        };
    }
}
