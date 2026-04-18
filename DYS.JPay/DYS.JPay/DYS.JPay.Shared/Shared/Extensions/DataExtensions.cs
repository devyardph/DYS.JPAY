using DYS.JPay.Shared.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Extensions
{
    public static class DataExtensions
    {
        public static string GetCategoryName(this List<CategoryDto> categories,Guid? id)
        {
            var category= categories.FirstOrDefault(c => c.Id == id);
            return category?.Name ?? string.Empty;
        }

    }
}
