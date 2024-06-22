using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HangfireAPI.Models;

namespace HangfireAPI.Data
{
    public class HangfireAPIContext : DbContext
    {
        public HangfireAPIContext (DbContextOptions<HangfireAPIContext> options)
            : base(options)
        {
        }

        public DbSet<HangfireAPI.Models.User> User { get; set; } = default!;
    }
}
