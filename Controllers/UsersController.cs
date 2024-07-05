﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HangfireAPI.Data;
using HangfireAPI.Models;
using Hangfire;
using HangfireAPI.Services;

namespace HangfireAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ResponseCache(Duration = 60)]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _service;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUsersService service, ILogger<UsersController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("/seed")]
        public async Task<ActionResult> SeedUsers()
        {
            await _service.SeedUsers();
            return Ok("Users has been seeded");
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetThenDeleteUser()
        {
            var users = await _service.GetThenDeleteUser();

            if (!users.Any())
            {
                return NotFound("No users found");
            }

            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _service.GetUser(id);
            if (user == null)
            {
                return NotFound("No user found");
            }

            return Ok(user);
        }

        [HttpGet("/spUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersSP(CancellationToken cancellationToken)
        {
            var users = await _service.GetUsersSP(cancellationToken);
            return users.Any() ? Ok(users) : NotFound("No users found");
        }

        [HttpGet("/spUser/{id}")]
        public async Task<ActionResult<User>> GetUserSP(int id, CancellationToken cancellationToken)
        {
            var user = await _service.GetUserSP(id, cancellationToken);
            return user != null ? Ok(user) : NotFound("No user found");
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            await _service.PutUser(id, user);
            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            var createdUser = await _service.PostUser(user);
            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _service.DeleteUser(id);
            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _service.UserExists(id);
        }
    }
}
