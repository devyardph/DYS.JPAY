using DYS.JPay.Shared.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Dtos
{  public class LoggerDto: BaseEntityDto
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public string ExecutedBy { get; set; }
    }

}
