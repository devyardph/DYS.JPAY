using DYS.JPay.Shared.Shared.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Services
{
    public class ResourceService: IResourceService
    {
        public async Task<string> ReadFileAsync(string path)
        {
            // Ensure the file is marked as <MauiAsset> in your .csproj
            // Example: <MauiAsset Include="wwwroot\template\daily_sales_report.html" />
            using var stream = await FileSystem.OpenAppPackageFileAsync(path);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}
