using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface IAccountService: IBaseService
    {
        Task<int> RegisterUserAsync(string username, string code, string role);
        Task<User> LoginAsync(string username, string code);
        Task<bool> IsInRoleAsync(User user, string role);
        Task GetAllAsync();
    }

    public class AccountService : BaseService, IAccountService
    {
        private readonly IRepository<User> _repo;

        public AccountService(IRepository<User> repo)
        {
            _repo = repo;
        }

        public async Task<int> RegisterUserAsync(string username, string code, string role)
        {
            if (code.Length != 6 || !code.All(char.IsDigit))
                throw new ArgumentException("Code must be a 6-digit number.");

            var user = new User { Username = username, Code = code, Role = role };
            return await _repo.InsertAsync(user);
        }

        public async Task<User> LoginAsync(string username, string code)
        {
            var users = await _repo.GetAllAsync();
            return users.FirstOrDefault(u => u.Username == username && u.Code == code);
        }

        public async Task GetAllAsync()
        {
            var users = await _repo.GetAllAsync();
        }

        public Task<bool> IsInRoleAsync(User user, string role)
        {
            return Task.FromResult(user?.Role == role);
        }
    }

}
