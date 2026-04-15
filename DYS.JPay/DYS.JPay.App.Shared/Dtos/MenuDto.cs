using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DYS.JPay.Common.Dtos
{
    public class MenuDto: SubMenuDto
    {
        public string Icon { get; set; }

        public List<SubMenuDto> SubMenus { get; set; } = new List<SubMenuDto>();
    }

    public class SubMenuDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Path { get; set; }
        public string Style { get; set; }
        public bool Active { get; set; } = false;
        public string Access { get; set; }
    }
}
