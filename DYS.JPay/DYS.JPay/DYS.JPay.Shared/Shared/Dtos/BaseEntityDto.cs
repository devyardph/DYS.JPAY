using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Dtos
{
    public class BaseEntityDto
    {
        public Guid? Id { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}
