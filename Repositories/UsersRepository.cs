using Hangfire;
using HangfireAPI.Data;
using HangfireAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace HangfireAPI.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly HangfireAPIContext _context;
    private readonly ILogger<UsersRepository> _logger;

    public UsersRepository(HangfireAPIContext context, ILogger<UsersRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task DeleteAll()
    {
        await _context.User.ExecuteDeleteAsync();
    }

    public async Task DeleteUser(int id)
    {
        var user = await _context.User.FindAsync(id);
        if (user == null)
        {
            throw new Exception($"No user found with id: {id}");
        }

        _context.User.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetThenDeleteUser()
    {
            BackgroundJob.Enqueue(() =>
            Console.WriteLine("Thanks for getting the userlist! Unfortunately the list wil get deleted in 10 seconds :("));

            var deleteJob = BackgroundJob.Schedule(() => DeleteAll(),
            TimeSpan.FromSeconds(10));

            BackgroundJob.ContinueJobWith(deleteJob, () => Console.WriteLine("List has been deleted!"));
            return await _context.User.ToListAsync();
    }

    public async Task<User> GetUser(int id)
    {
        var user = await _context.User.FindAsync(id);

        if (user == null)
        {
            throw new Exception($"No user found with id: {id}");
        }

        return user;
    }

    public async Task<User> PostUser(User user)
    {
        _context.User.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task PutUser(int id, User user)
    {
        if (id != user.Id)
        {
            throw new Exception("");
        }

        _context.Entry(user).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
            {
                throw new Exception($"No user found with id: {id}");
            }
            else
            {
                throw;
            }
        }
    }

    public bool UserExists(int id)
    {
        return _context.User.Any(e => e.Id == id);
    }
}
