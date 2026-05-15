using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Dtos
{
    public class OutputDto<T> where T: class
    {
        public T? Entity { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; } = false;
    }
}
