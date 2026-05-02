using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;


namespace DYS.JPay.Shared.Features.Logs.ViewModels
{
    public partial class LogsViewModel : BaseViewModel
    {

        public readonly ILoggerService _logService;

        public LogsViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            SessionService sessionService,
            ILoggerService logService)
            : base(navigationManager, jsRuntime, sessionService)
        {
            _logService = logService;
        }

        #region PROPERTIES
        [ObservableProperty]
        private SearchDto search = new SearchDto();

        [ObservableProperty]
        private PageDto<LoggerDto> loggers = new PageDto<LoggerDto>() { Results = new List<LoggerDto>() };
        [ObservableProperty]
        private LoggerDto logger = new LoggerDto();
        #endregion


        #region FUNCTIONS
        public async Task SearchLogsWithPagingAsync(string action = "", bool refresh = false)
        {
            IsBusy = true;
            var currentPage = refresh || Search.CurrentPage == 0 ? 1 : Search.CurrentPage;
            if (action == "next") currentPage = Search.NextEnabled ? Search.CurrentPage + 1 : Search.CurrentPage;
            else if (action == "previous") currentPage = Search.PreviousEnabled ? Search.CurrentPage - 1 : Search.CurrentPage;

            Loggers = new PageDto<LoggerDto>();
            Search.CurrentPage = currentPage;
            Search.PageSize = 20;
            Search.Columns = new List<string>() { $"Name","Email", "Role" };
            var output = await _logService.GetLogsAsync(Search);
            if (output is not null)
            {

                Loggers = output.Adapt<PageDto<LoggerDto>>();
                Search.CurrentPage = Loggers.PageIndex;

                var display = Loggers!.PageIndex * Search!.PageSize;
                var show = Loggers!.TotalCount >= display ? display : Loggers.TotalCount;
                Search.PreviousEnabled = Loggers.PageIndex > 1;
                Search.NextEnabled = Loggers.PageIndex <= Loggers.TotalCount && show < Loggers.TotalCount;
                Search.Summary = $"showing {show} of {Loggers!.TotalCount.ToString("N0")} patients";
            }
            IsBusy = false;
        }

        public async Task SubmitUserAsync()
        {
            IsBusy = true;
            var logger = Logger.Adapt<Logger>();
            await _logService.SaveAsync(logger);
            await SearchLogsWithPagingAsync();
            await _jsRuntime.InvokeVoidAsync("closeOffcanvas", "user-overlay", "user-component");
            IsBusy = false;
        }

        public async Task OpenLogger(LoggerDto? logger)
        {
            Logger = logger ?? new LoggerDto();
            await _jsRuntime.InvokeVoidAsync("openOffcanvas","user-overlay","logger-component");
        }
        #endregion

    }
}
