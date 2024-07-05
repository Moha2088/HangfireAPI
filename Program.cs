using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HangfireAPI.Data;
using Hangfire;
using System.Configuration;
using HangfireAPI.Repositories;
using HangfireAPI.Services;
using HangfireBasicAuthenticationFilter;
using HangfireAPI.Authorization;

namespace HangfireAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<HangfireAPIContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("HangfireAPIContext") ?? throw new InvalidOperationException("Connection string 'HangfireAPIContext' not found.")));

        var hangfireCon = builder.Configuration.GetConnectionString("HangfireCon");

        // Add services to the container.

        builder.Services.AddResponseCaching();
        builder.Services.AddScoped<IUsersRepository, UsersRepository>();
        builder.Services.AddScoped<IUsersService, UsersService>();
        builder.Services.AddTransient<Timeservice>();
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // HangFire Client Configuration
        builder.Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(hangfireCon));

        // Hangfire Server Configuration
        builder.Services.AddHangfireServer();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseResponseCaching();
        app.UseHttpsRedirection();

        app.UseHangfireDashboard(options: new DashboardOptions
        {
            Authorization = new[]
            {
                new AuthorizationFilter()
            },

            DarkModeEnabled = false,
            DashboardTitle = "HangfireAPI Dashboard"
        });

        app.UseAuthorization();

        var timeService = app.Services.GetRequiredService<Timeservice>();
        BackgroundJob.Enqueue(() => Console.WriteLine("Hello from Hangfire!"));
        RecurringJob.AddOrUpdate("DisplayTime", () => timeService.DisplayTime(), Cron.Minutely);


        app.MapControllers();

        app.Run();
    }
}
