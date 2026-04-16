using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Dtos
{
    public class CartDto
    {
        public TransactionDto Transaction { get; set; }
        public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
    }
}
