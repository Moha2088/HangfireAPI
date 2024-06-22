using HangfireAPI.Models;

namespace HangfireAPI.Services;

public interface IUsersService
{
    Task DeleteUser(int id);
    Task<IEnumerable<User>> GetThenDeleteUser();
    Task<User> GetUser(int id);
    Task<User> PostUser(User user);
    Task PutUser(int id, User user);
    bool UserExists(int id);
}