using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Dtos
{
    public class UserDto 
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Code { get; set; } // 6-digit login code
        public string Role { get; set; } // e.g., "Admin", "Cashier"
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
