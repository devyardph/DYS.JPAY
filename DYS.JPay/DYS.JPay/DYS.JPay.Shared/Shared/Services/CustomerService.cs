using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface ICustomerService: IBaseService
    {
        Task<int> AddCustomerAsync(Customer customer);
    }

    public class CustomerService : BaseService,ICustomerService
    {
        private readonly IRepository<Customer> _customerRepository;

        public CustomerService(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public Task<int> AddCustomerAsync(Customer customer) => _customerRepository.InsertAsync(customer);
    }


}
