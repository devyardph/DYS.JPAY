using DYS.JPay.Shared.Features.Transactions.Views;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using DYS.JPay.Shared.Shared.Helpers;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.Settings;

namespace DYS.JPay.Schedulers
{
    public class DailyJob
    {
        private readonly System.Timers.Timer _timer;
        private readonly IAppSettingService _appSettingService;
        private readonly ILoggerService _loggerService;
        public DailyJob(
            IAppSettingService appSettingService,
            ILoggerService loggerService)
        {
            _appSettingService = appSettingService;
            _loggerService = loggerService;
            // Run once every 24 hours2
            _timer = new System.Timers.Timer(TimeSpan.FromHours(2).TotalMilliseconds);
            _timer.Elapsed += async (s, e) => await RunDailyExport();
            _timer.AutoReset = true;
            _timer.Start();
        }

        private async Task RunDailyExport()
        {
            var info = new Logger()
            {
                Type = GlobalSettings.INFO,
                Message = "Started Daily Sales Report."
            };
            await _loggerService.SaveAsync(info);

            var data = new List<TransactionDto>();
            var bytes = CsvHelpers.ExportToCsv(data);
            var base64 = Convert.ToBase64String(bytes);

            string today = DateTime.Now.ToString("yyyyMMdd");
            string filePath = Path.Combine(FileSystem.AppDataDirectory, $"sales_{today}.csv");

            var setting = await _appSettingService.GetSettingAsync();
            var sender = setting.GmailAccount;
            var password = setting.AppPassword;
            var emailNotificationEnabled = setting.ReceiveEmailNotification;

            if (emailNotificationEnabled &&
                !string.IsNullOrEmpty(sender) &&
                !string.IsNullOrEmpty(password))
            {
                var emailSent = await EmailService.SendEmailAsync(
                    "jfvaleroso.smart@gmail.com",
                    $"Daily Sales Report {today}",
                    "Attached is the daily sales CSV.", sender, password
                );

                var log = new Logger()
                {
                    Type = emailSent.Success ? GlobalSettings.INFO : GlobalSettings.ERROR,
                    Message = emailSent.Message
                };
                await _loggerService.SaveAsync(log);
            }
        }
    }
}
