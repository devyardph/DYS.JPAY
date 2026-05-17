using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Extensions;
using DYS.JPay.Shared.Shared.Helpers;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.Settings;
using System.ComponentModel.Design;
using System.Text;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface ISchedulerService: IBaseService
    {
        Task<ResponseDto> RunDailyExport(CancellationToken cancellationToken = default);
        Task<ResponseDto> GenerateSalesReport(EmailDto email,
                                         SearchReportDto parameter,
                                         List<TransactionDto> transactions,
                                         CancellationToken cancellationToken = default);
    }

    public class SchedulerService: BaseService, ISchedulerService
    {
       
        private readonly IAppSettingService _appSettingService;
        private readonly ILoggerService _loggerService;
        private readonly ITransactionService _transactionService;
        private readonly IUserService _userService;
        private readonly IResourceService _resourceService;
        private readonly SessionService _sessionService;
        public SchedulerService(
            IAppSettingService appSettingService,
            ILoggerService loggerService,
            ITransactionService transactionService,
            IResourceService resourceService,
            IUserService userService,
            SessionService sessionService)
        {
            _appSettingService = appSettingService;
            _loggerService = loggerService;
            _transactionService = transactionService;
            _userService = userService;
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

            var now = DateTime.UtcNow;
            var startDate = now.StartOfDay();
            var endDate = now.EndOfDay();
            var transactions = await _transactionService.GetAllTransactionsAsync(query =>
                query.DateCreated <= endDate &&
                query.DateCreated >= startDate);

            var template = await _resourceService.ReadFileAsync("wwwroot/templates/daily_sales_report.html");
            var setting = await _appSettingService.GetSettingAsync();
            var sender = setting.GmailAccount;
            var password = setting.AppPassword;
            var emailNotificationEnabled = setting.ReceiveEmailNotification;

            template = template.Replace("#store-name#", setting?.StoreName);
            template = template.Replace("#store-branch#", setting?.Branch);
            template = template.Replace("#store-counter#", setting?.Counter);
            template = template.Replace("#date#", now.ToString("dd MMM yyyy"));

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
            var bytes = CsvHelpers.ExportToCsv(transactions ?? new List<Entities.Transaction>());

            var owner = await _userService.GetUserAsync(query => query.Role == GlobalSettings.OWNER);

            if (emailNotificationEnabled &&
                !string.IsNullOrEmpty(sender) &&
                !string.IsNullOrEmpty(password) &&
                !string.IsNullOrEmpty(owner?.Email))
            {
                var emailSent = await EmailService.SendEmailAsync(
                   $"{owner?.Email?.Trim()}",
                   $"Daily Sales Report : {now.FormatDate("dd MMM yyyy")}",
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

        public async Task<ResponseDto> GenerateSalesReport(EmailDto email,
                                                      SearchReportDto parameter,
                                                      List<TransactionDto> transactions, 
                                                      CancellationToken cancellationToken = default)
        {
            var output = new ResponseDto();
            var info = new Logger()
            {
                Type = GlobalSettings.INFO,
                Message = "Manually generated sales report."
            };
            await _loggerService.SaveAsync(info);

            var bytes = CsvHelpers.ExportToCsv(transactions);

            string today = DateTime.Now.ToString("yyyyMMdd");
            var template = await _resourceService.ReadFileAsync("wwwroot/templates/generated_sales_report.html");

            var setting = await _appSettingService.GetSettingAsync();
            var sender = setting.GmailAccount;
            var password = setting.AppPassword;
            var emailNotificationEnabled = setting.ReceiveEmailNotification;

            template = template.Replace("#store-name#", setting?.StoreName);
            template = template.Replace("#store-branch#", setting?.Branch);
            template = template.Replace("#store-counter#", setting?.Counter);
            template = template.Replace("#start-date#", parameter?.StartDate.ToLocalTime().FormatDate("MMM dd yyyy"));
            template = template.Replace("#end-date#", parameter?.EndDate.ToLocalTime().FormatDate("MMM dd yyyy"));

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
                    $"{email.Subject}",
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
