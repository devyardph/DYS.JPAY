using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Code { get; set; } // 6-digit login code
        public string Role { get; set; } // e.g., "Admin", "Cashier"
        public string Email { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
