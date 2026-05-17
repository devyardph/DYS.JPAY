using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Extensions;
using DYS.JPay.Shared.Shared.Helpers;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.Settings;
using System.ComponentModel.Design;
using System.Text;
using System.Transactions;

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
        private readonly IResourceService _resourceService;
        private readonly SessionService _sessionService;
        public SchedulerService(
            IAppSettingService appSettingService,
            ILoggerService loggerService,
            IResourceService resourceService,
            SessionService sessionService)
        {
            _appSettingService = appSettingService;
            _loggerService = loggerService;
            _resourceService = resourceService;
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

            // Namespace + folder + filename
            //string basePath = AppContext.BaseDirectory;
            //string filePath = Path.Combine(basePath, "wwwroot", "template", "daily_sales_report.html");
            //var template = await File.ReadAllTextAsync(filePath);

            var template = await _resourceService.ReadFileAsync("wwwroot/templates/daily_sales_report.html");

            var setting = await _appSettingService.GetSettingAsync();
            var sender = setting.GmailAccount;
            var password = setting.AppPassword;
            var emailNotificationEnabled = setting.ReceiveEmailNotification;

            template = template.Replace("#store-name#", setting?.StoreName);
            template = template.Replace("#store-branch#", setting?.Branch);
            template = template.Replace("#store-counter#", setting?.Counter);

            var content = new StringBuilder();
            foreach (var transaction in transactions)
            {
                content.Append("<tr style='border-bottom:1px solid #eee;'>");
                content.Append($"<td align='left'>{transaction.Code}</td>");
                content.Append($"<td align='left'>{transaction.CustomerName}</td>");
                content.Append($"<td align='center'>{transaction.Count}</td>");
                content.Append($"<td align='right'>{setting?.Currency}{transaction.GrandTotal?.ToString("N2")}</td>");
                content.Append($"<td align='left'>{transaction.DateCreated.FormatDate("dd-MM-yy hh:mmtt")}</td>");
                content.Append($"<td align='center'>{transaction.Status}</td>");
                content.Append("</tr>");
            }
            template = template.Replace("#transaction-items#", content.ToString());
            template = template.Replace("#grand-total#", $"{setting?.Currency}{transactions?.Sum(query => query.GrandTotal ?? 0).ToString("N2")}");
            template = template.Replace("#completed#", transactions?.Where(query => query.Status == GlobalSettings.COMPLETED).Count().ToString());
            template = template.Replace("#pending#", transactions?.Where(query => query.Status == GlobalSettings.PREPARING).Count().ToString());
            template = template.Replace("#cancelled#", transactions?.Where(query => query.Status == GlobalSettings.CANCELLED).Count().ToString());

            if (emailNotificationEnabled &&
                !string.IsNullOrEmpty(sender) &&
                !string.IsNullOrEmpty(password))
            {
                var emailSent = await EmailService.SendEmailAsync(
                    $"{email.Email.Trim()}",
                    $"{email.Subject} {today}",
                    $"{template}", sender, password,
                    bytes,
                    cancellationToken
                );

                output.Success = emailSent.Success;
                output.Message = emailSent.Message;

                info = new Logger();
                info.Type = emailSent.Success ? GlobalSettings.INFO : GlobalSettings.ERROR;
                info.Message = emailSent.Message;
                info.ExecutedBy = _sessionService.CurrentUser.Username;
                info.DateCreated = DateTime.UtcNow;
            }
            else
            {
                output.Success = false;
                output.Message = "Missing email config.";
                info = new Logger();
                info.Type = GlobalSettings.ERROR;
                info.Message = output.Message;
                info.DateCreated = DateTime.UtcNow;
                info.ExecutedBy = _sessionService.CurrentUser.Username;
            }
            await _loggerService.SaveAsync(info);
            return output;
        }
    }
}
