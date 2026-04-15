using DYS.JPay.Common.Dtos;
using DYS.JPay.Server.Shared.Entities;
using DYS.JPay.Server.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface ITestingService
    {
        Task<List<Testing>> GetAllAsync();
        Task<int> SaveChangesAsync(Testing appSetting);
    }
    public class TestingService : ITestingService
    {
        private readonly IRepository<Testing> _testRepository;

        public TestingService(IRepository<Testing> testRepository)
        {
            _testRepository = testRepository;
        }

        public Task<List<Testing>> GetAllAsync() =>
            _testRepository.GetAllAsync();

        public async Task<int> SaveChangesAsync(Testing appSetting) =>
           await _testRepository.InsertAsync(appSetting);
    }

}
