using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Helpers;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.Settings;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface ISchedulerService: IBaseService
    {
        Task RunDailyExport(CancellationToken cancellationToken = default);
    }

    public class SchedulerService: BaseService, ISchedulerService
    {
       
        private readonly IAppSettingService _appSettingService;
        private readonly ILoggerService _loggerService;
        public SchedulerService(
            IAppSettingService appSettingService,
            ILoggerService loggerService)
        {
            _appSettingService = appSettingService;
            _loggerService = loggerService;

        }

        public async Task RunDailyExport(CancellationToken cancellationToken = default)
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
            //string filePath = Path.Combine(FileSystem.AppDataDirectory, $"sales_{today}.csv");

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
                    "Attached is the daily sales CSV.", sender, password, 
                    cancellationToken
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
