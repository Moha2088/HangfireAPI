using HangfireAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace HangfireAPI.Repositories;

public interface IUsersRepository
{
    Task DeleteUser(int id);
    Task SeedUsers();
    Task<UserResponse> GetUsers(int pageNumber);
    Task<User> GetUser(int id);
    Task<IEnumerable<User>> GetUsersSP(CancellationToken cancellationToken);
    Task <User> GetUserSP(int id, CancellationToken cancellationToken);
    Task<User> PostUser(User user);
    Task PutUser(int id, User user);
    bool UserExists(int id);
}