using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DYS.JPay.Common.Extensions
{
    public static class BooleanExtensions
    {

        public static string GetConditionalString(bool input, string output1, string output2)
        {
            return input == true? output1 : output2;
        }

        public static bool ContainsKeywords(string[] keywords, string value)
        {
            return keywords.Any(keyword => value.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }
    }
}
