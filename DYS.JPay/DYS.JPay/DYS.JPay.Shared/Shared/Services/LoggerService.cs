using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface ILoggerService : IBaseService
    {
        Task<PageDto<Logger>> GetLogsAsync(SearchDto search);
        Task<int> SaveAsync(Logger logger);
    }
    public class LoggerService : BaseService, ILoggerService
    {
        private readonly IRepository<Logger> _loggerRepository;

        public LoggerService(IRepository<Logger> loggerRepository)
        {
            _loggerRepository = loggerRepository;
        }

        public async Task<PageDto<Logger>> GetLogsAsync(SearchDto search) =>
             await _loggerRepository.GetPagedAsync(search.CurrentPage,
                 search.PageSize,
                 search.Keyword,
                 search.Columns,
                 showAll: false);

        public async Task<int> SaveAsync(Logger logger) =>
           await _loggerRepository.InsertAsync(logger);
    }

}
