using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface IAppSettingService: IBaseService
    {
        Task<AppSetting> GetSettingAsync();
        Task<int> SaveChangesAsync(AppSetting appSetting);
    }
    public class AppSettingService : BaseService,IAppSettingService
    {
        private readonly IRepository<AppSetting> _appSettingRepository;

        public AppSettingService(IRepository<AppSetting> appSettingRepository)
        {
            _appSettingRepository = appSettingRepository;
        }

        public Task<AppSetting> GetSettingAsync() =>
            _appSettingRepository.GetAsync(query => query.Default == true);

        public async Task<int> SaveChangesAsync(AppSetting appSetting) =>
           await _appSettingRepository.UpdateAsync(appSetting);
    }

}
