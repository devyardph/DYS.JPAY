using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Dtos
{
    public class SubscriptionPlanDto
    {
        public Guid? Id { get; set; }
        public string Code { get; set; }
        public string Tier { get; set; }
        public string Price { get; set; }
        public int Order { get; set; }
    }
}
