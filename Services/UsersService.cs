using HangfireAPI.Models;
using HangfireAPI.Repositories;

namespace HangfireAPI.Services;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _repository;

    public UsersService(IUsersRepository repository)
    {
        _repository = repository;
    }



    public async Task DeleteUser(int id)
    {
       await _repository.DeleteUser(id);
    }

    public async Task<IEnumerable<User>> GetThenDeleteUser()
    {
        return await _repository.GetThenDeleteUser();
    }

    public Task<User> GetUser(int id)
    {
        return _repository.GetUser(id);
    }

    public Task<User> PostUser(User user)
    {
        return _repository.PostUser(user);
    }

    public async Task PutUser(int id, User user)
    {
        await _repository.PutUser(id, user);
    }

    public Task SeedUsers()
    {
        return _repository.SeedUsers();
    }

    public bool UserExists(int id)
    {
       return _repository.UserExists(id);
    }
}
