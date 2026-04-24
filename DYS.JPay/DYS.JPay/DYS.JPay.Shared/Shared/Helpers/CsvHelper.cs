using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DYS.JPay.Shared.Shared.Helpers
{
    public static class CsvHelper
    {
        public static byte[] ExportToCsv<T>(IEnumerable<T> records)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(records);
            writer.Flush();

            return memoryStream.ToArray();
        }
    }
}
