using HangfireAPI.Models;
using HangfireAPI.Repositories;

namespace HangfireAPI.Services;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _repository;
    private readonly ILogger<UsersService> _logger;

    public UsersService(IUsersRepository repository, ILogger<UsersService> logger)
    {
        _repository = repository;
        _logger = logger;
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

    public Task<IEnumerable<User>> GetUsersSP(CancellationToken cancellationToken)
    {
        return _repository.GetUsersSP(cancellationToken);
    }

    public Task<User> GetUserSP(int id, CancellationToken cancellationToken)
    {
        return _repository.GetUserSP(id, cancellationToken);
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
