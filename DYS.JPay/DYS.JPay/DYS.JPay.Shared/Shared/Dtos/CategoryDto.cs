using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Dtos
{
    public class CategoryDto : BaseEntityDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; } = true;
    }
}
