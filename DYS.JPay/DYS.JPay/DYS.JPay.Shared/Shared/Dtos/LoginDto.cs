using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Dtos
{
    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Passcode { get; set; } = string.Empty;
    }
}
