using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Settings;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
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

        public static string GetDefaultName(this List<SelectDto> data, string id)
        {
            var option = data.FirstOrDefault(c => c.Id == id);
            return option?.Name ?? string.Empty;
        }

        public static string GetDisplayName(this List<SelectDto> data, string id)
        {
            var option = data.FirstOrDefault(c => c.Id == id);
            return option?.DisplayName ?? string.Empty;
        }

        public static StatusDto GetTransactionStatus(string content)
        {
            var status = new StatusDto();
            if (content == GlobalSettings.NEW)
            {
                status.Title = GlobalSettings.NEW;
                status.TextColor = "#0d47a1";       // deep blue text
                status.BackgroundColor = "#bbdefb";
            }
            else if (content == GlobalSettings.PREPARING)
            {
                status.Title = GlobalSettings.PREPARING;
                status.TextColor = "#8a6d3b";       // brownish text
                status.BackgroundColor = "#fff3cd"; // soft yellow background
            }
            else if (content == GlobalSettings.COMPLETED)
            {
                status.Title = GlobalSettings.COMPLETED;
                status.TextColor = "#155724";       // dark green text
                status.BackgroundColor = "#d4edda"; // light green background
            }
            else if (content == GlobalSettings.CANCELLED)
            {
                status.Title = GlobalSettings.CANCELLED;
                status.TextColor = "#721c24";       // dark red text
                status.BackgroundColor = "#f8d7da"; // light red background
            }
            //else if (content == GlobalSettings.PAYMENT_PENDING)
            //{
            //    status.Title = GlobalSettings.PAYMENT_PENDING;
            //    status.TextColor = "#0c5460";       // dark cyan text
            //    status.BackgroundColor = "#d1ecf1"; // light cyan background
            //}
            //else if (content == GlobalSettings.PAID)
            //{
            //    status.Title = GlobalSettings.PAID;
            //    status.TextColor = "#1b1e21";       // dark gray text
            //    status.BackgroundColor = "#c3e6cb"; // mint green background
            //}
            return status;
        }

    }
}
