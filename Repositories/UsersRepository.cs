using Hangfire;
using HangfireAPI.Data;
using HangfireAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HangfireAPI.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly HangfireAPIContext _context;
    private readonly ILogger<UsersRepository> _logger;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public UsersRepository(HangfireAPIContext context, ILogger<UsersRepository> logger, IBackgroundJobClient backgroundJobClient)
    {
        _context = context;
        _logger = logger;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task DeleteAll()
    {
        await _context.User.ExecuteDeleteAsync();
    }

    public void SeedData()
    {
        _context.User.AddRange(

            new User
            {
                Name = "TestUser1",
                Password = "TestPassword"
            },

             new User
             {
                 Name = "TestUser2",
                 Password = "TestPassword"
             },

              new User
              {
                  Name = "TestUser3",
                  Password = "TestPassword"
              },

               new User
               {
                   Name = "TestUser4",
                   Password = "TestPassword"
               },

                new User
                {
                    Name = "TestUser5",
                    Password = "TestPassword"
                }
            );

        _context.SaveChanges();
    }

    public async Task SeedUsers()
    {
        _backgroundJobClient.Enqueue(() => Console.WriteLine("Initiating seeding of the database!"));
        var seedJobId = _backgroundJobClient.Schedule(() => SeedData(), TimeSpan.FromSeconds(5));
        _backgroundJobClient.ContinueJobWith(seedJobId, () => Console.WriteLine("Data has been seeded"));
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
        _backgroundJobClient.Enqueue(() =>
        Console.WriteLine("Thanks for getting the userlist! Unfortunately the list wil get deleted in 10 seconds :("));

        var deleteJobId = _backgroundJobClient.Schedule(() => DeleteAll(),
        TimeSpan.FromSeconds(10));

        _backgroundJobClient.ContinueJobWith(deleteJobId, () => Console.WriteLine("List has been deleted!"));
        return await _context.User.ToListAsync();
    }

    public async Task<IEnumerable<User>> GetUsersSP(CancellationToken cancellationToken)
    {
        string storedProcedure = "spGetUsers";

        try
        {
            var users = await _context.User
            .FromSqlInterpolated($"{storedProcedure}")
            .ToListAsync(cancellationToken);

            return users;
        }

        catch (OperationCanceledException ex)
        {
            _logger.LogError($"The request has been cancelled : {ex.Message}");
            return Enumerable.Empty<User>();
        }
    }

    public async Task<User> GetUserSP(int id, CancellationToken cancellationToken)
    {
        string storedProcedure = "spGetUser";

        try
        {
            var getUserQuery = await _context.User
            .FromSqlInterpolated($"{storedProcedure} {id}")
            .ToListAsync(cancellationToken);

            return getUserQuery.Single();
        }

        catch (OperationCanceledException ex)
        {
            _logger.LogInformation($"The request has been cancelled: {ex.Message}");
            return (User)Enumerable.Empty<User>();
        }
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
