using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Helpers;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.Settings;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface ISchedulerService: IBaseService
    {
        Task<ResponseDto> RunDailyExport(CancellationToken cancellationToken = default);
        Task<ResponseDto> RunDailyExport(EmailDto email,
                                         List<TransactionDto> transactions,
                                         CancellationToken cancellationToken = default);
    }

    public class SchedulerService: BaseService, ISchedulerService
    {
       
        private readonly IAppSettingService _appSettingService;
        private readonly ILoggerService _loggerService;
        private readonly SessionService _sessionService;
        public SchedulerService(
            IAppSettingService appSettingService,
            ILoggerService loggerService,
            SessionService sessionService)
        {
            _appSettingService = appSettingService;
            _loggerService = loggerService;
            _sessionService = sessionService;
        }

        public async Task<ResponseDto> RunDailyExport(CancellationToken cancellationToken = default)
        {
            var output = new ResponseDto();
            var info = new Logger()
            {
                Type = GlobalSettings.INFO,
                Message = "Daily Sales Report triggered."
            };
            await _loggerService.SaveAsync(info);

            var data = new List<TransactionDto>();
            var bytes = CsvHelpers.ExportToCsv(data);
            //var base64 = Convert.ToBase64String(bytes);

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
                    bytes,
                    cancellationToken
                );

                output.Success = emailSent.Success;
                output.Message = emailSent.Message;

                info = new Logger();
                info.Type = emailSent.Success ? GlobalSettings.INFO : GlobalSettings.ERROR;
                info.Message = emailSent.Message;
                info.ExecutedBy = _sessionService.CurrentUser.Username;
            }
            else
            {
                output.Success = false;
                output.Message = "Missing email config.";
                info = new Logger();
                info.Type = GlobalSettings.ERROR;
                info.Message = output.Message;
                info.ExecutedBy = _sessionService.CurrentUser.Username;
            }
            await _loggerService.SaveAsync(info);
            return output;
        }

        public async Task<ResponseDto> RunDailyExport(EmailDto email,
                                                      List<TransactionDto> transactions, 
                                                      CancellationToken cancellationToken = default)
        {
            var output = new ResponseDto();
            var info = new Logger()
            {
                Type = GlobalSettings.INFO,
                Message = "Daily Sales Report triggered."
            };
            await _loggerService.SaveAsync(info);

            var bytes = CsvHelpers.ExportToCsv(transactions);

            string today = DateTime.Now.ToString("yyyyMMdd");

            var setting = await _appSettingService.GetSettingAsync();
            var sender = setting.GmailAccount;
            var password = setting.AppPassword;
            var emailNotificationEnabled = setting.ReceiveEmailNotification;

            if (emailNotificationEnabled &&
                !string.IsNullOrEmpty(sender) &&
                !string.IsNullOrEmpty(password))
            {
                var emailSent = await EmailService.SendEmailAsync(
                    $"{email.Email.Trim()}",
                    $"{email.Subject} {today}",
                    $"{email.Subject}", sender, password,
                    bytes,
                    cancellationToken
                );

                output.Success = emailSent.Success;
                output.Message = emailSent.Message;

                info = new Logger();
                info.Type = emailSent.Success ? GlobalSettings.INFO : GlobalSettings.ERROR;
                info.Message = emailSent.Message;
                info.ExecutedBy = _sessionService.CurrentUser.Username;
            }
            else
            {
                output.Success = false;
                output.Message = "Missing email config.";
                info = new Logger();
                info.Type = GlobalSettings.ERROR;
                info.Message = output.Message;
                info.ExecutedBy = _sessionService.CurrentUser.Username;
            }
            await _loggerService.SaveAsync(info);
            return output;
        }
    }
}
