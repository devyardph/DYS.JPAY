using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Repositories;
using Mapster;
namespace DYS.JPay.Shared.Shared.Services
{
    public interface IUserService : IBaseService
    {
        Task<PageDto<User>> GetUsersAsync(SearchDto search);
        Task<User> GetUserByIdAsync(Guid id);
        Task<User> SubmitUserAsync(UserDto user);
    }
    public class UserService : BaseService, IUserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PageDto<User>> GetUsersAsync(SearchDto search)  =>
             await _userRepository.GetPagedAsync(search.CurrentPage, 
                 search.PageSize, 
                 search.Keyword, 
                 search.Columns);

        public async Task<User> GetUserByIdAsync(Guid id) =>
            await _userRepository.GetAsync(query => query.Id == id);
        public async Task<User> SubmitUserAsync(UserDto user)
        {
            var item = user.Adapt<User>();
            if (user.Id == Guid.Empty ||
                user.Id == null)
            {
                item.Id = Guid.NewGuid();
                await _userRepository.InsertAsync(item);
            }
            else
            {
                await _userRepository.UpdateAsync(item);
            }
            return item;
        }
    }

}
