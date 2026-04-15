using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DYS.JPay.Shared.Shared.Dtos
{
    public class SearchDto
    {
        public Guid? Id { get; set; } 
        public string Keyword { get; set; } = string.Empty;
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public List<string> Columns { get; set; } //columns to search in


        public bool NextEnabled { get; set; } = false;
        public bool PreviousEnabled { get; set; } = false;
        public string Summary { get; set; } = string.Empty;
    }
}
