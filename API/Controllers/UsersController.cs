using System;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class UsersController : BaseApiController
{
    private readonly DataContext context;

    public UsersController(DataContext _context)
    {
        context = _context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        List<AppUser> users = await context.Users.ToListAsync();

        return users;
    }

    [HttpGet("{id:int}")] // /api/users/2
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        var user = await context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }
}
