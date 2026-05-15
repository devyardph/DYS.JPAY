using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Extensions;
using DYS.JPay.Shared.Shared.Repositories;
using DYS.JPay.Shared.Shared.Settings;
using Mapster;
using System.Linq.Expressions;
namespace DYS.JPay.Shared.Shared.Services
{
    public interface IUserService : IBaseService
    {
        Task<PageDto<User>> GetUsersAsync(SearchDto search);
        Task<User> GetUserByIdAsync(Guid id);
        Task<User> SubmitUserAsync(UserDto user);
        Task<User> GetUserAsync(Expression<Func<User, bool>> predicate);
    }
    public class UserService : BaseService, IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Logger> _loggerRepository;
        private readonly SessionService _sessionService;

        public UserService(IRepository<User> userRepository,
            IRepository<Logger> loggerRepository,
            SessionService sessionService)
        {
            _userRepository = userRepository;
            _loggerRepository = loggerRepository;
            _sessionService = sessionService;

            _userRepository.EntityChanged += (s, e) =>
            {
                var logger = new Logger
                {
                    Type = GlobalSettings.INFO,
                    Message = $"{e.Action} entity of type {typeof(User).Name}:  {JsonExtensions.Convert(e.Entity)}",
                    DateCreated = DateTime.UtcNow,
                    ExecutedBy = _sessionService.CurrentUser?.Name ?? string.Empty
                };
                _loggerRepository.InsertAsync(logger);
            };
        }

        public async Task<PageDto<User>> GetUsersAsync(SearchDto search)  =>
             await _userRepository.GetPagedAsync(search.CurrentPage, 
                 search.PageSize, 
                 search.Keyword, 
                 search.Columns, showAll: false);

        public async Task<User> GetUserAsync(Expression<Func<User, bool>> predicate) =>
          await _userRepository.GetAsync(predicate);

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
