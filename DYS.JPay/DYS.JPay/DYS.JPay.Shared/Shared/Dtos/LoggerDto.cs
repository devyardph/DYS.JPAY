using DYS.JPay.Shared.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Dtos
{  public class LoggerDto
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public string ExxecutedBy { get; set; }
        public DateTime? DateExecuted { get; set; } = DateTime.UtcNow;
    }

}
