using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private DataContext context;
    public AccountController(DataContext _context)
    {
        context = _context;
    }

    [HttpPost("register")] // account/register
    public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
    {
        if (await UserExist(registerDto.Username))
        {
            return BadRequest("Username is already taken!");
        }

        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            UserName = registerDto.Username,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        return user;
    }

    private async Task<bool> UserExist(string username)
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower()); // Bob != bob
    }
}
