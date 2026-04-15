using DYS.JPay.Common.Dtos;
using DYS.JPay.Server.Shared.Entities;
using DYS.JPay.Server.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Server.Shared.Features.Dashboard.Services
{
    public interface ISampleService
    {
        Task<List<Testing>> GetAllAsync();
    }
    public class SampleService : ISampleService
    {
        private readonly IRepository<Testing> _testRepository;

        public SampleService(IRepository<Testing> testRepository)
        {
            _testRepository = testRepository;
        }

        public Task<List<Testing>> GetAllAsync() =>
            _testRepository.GetAllAsync();

    }

}
