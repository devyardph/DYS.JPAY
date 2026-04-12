using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Entities
{
    public class AppSetting : BaseEntity
    {
        public string StoreName { get; set; }
        public string StoreDescription { get; set; }
        public string Currency { get; set; }
        public string Display { get; set; } // grid or list
        public bool Default { get; set; } = false;
    }
}
