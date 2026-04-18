using DYS.JPay.Shared.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Dtos
{
    public class CartDto
    {
        public TransactionDto Transaction { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
