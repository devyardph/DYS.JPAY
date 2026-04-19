using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DYS.JPay.Shared.Shared.Dtos
{
    public class SelectDto
    {
        public string Parent { get; set; }
        public string Index { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
        public bool ReadOnly { get; set; }
        public string Style { get; set; }
        public bool Visible { get; set; } = true;
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string Note { get; set; }
        public string Icon { get; set; }
        public string Background { get; set; }
        public int Order { get; set; }
        public SelectDto()
        {
                
        }
        public SelectDto(string id, string name)
        {
           Id= id; 
           Name= name;   
           Description = name;
        }
    }
}
