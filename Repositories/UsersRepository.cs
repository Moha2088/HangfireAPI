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

    public void SeedData()
    {
        for (int i = 1; i < 50; i++)
        {
            _context.User.AddRange(
                new User
                {
                    Name = $"TestUser{i}",
                    Password = $"TestPassword{i}{i}"
                }
            );

            _context.SaveChanges();
        }
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

    public async Task<UserResponse> GetUsers(int pageNumber)
    {
        int pageItemCount = 10;
        int totalItemCount = _context.User.Count() / pageItemCount;

        var usersToDisplay = await _context.User    
            .Skip((pageNumber - 1) * pageItemCount)
            .Take(pageItemCount)
            .ToListAsync();

        int nextPageCount = pageNumber;
        int prevPageCount = pageNumber;
        int? nextPage = pageNumber < totalItemCount ? nextPageCount += 1 : null;
        int? prevPage = pageNumber - 1 != 0 ? prevPageCount -= 1 : null;


        return new UserResponse
        {
            Users = usersToDisplay,
            TotalUsers = _context.User.Count(),
            CurrentPage = pageNumber,
            Pages = totalItemCount,
            NextPage = nextPage,
            PrevPage = prevPage
        };
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
