using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private DataContext context;
    private ITokenService tokenService;

    private IMapper mapper;
    public AccountController(DataContext _context, ITokenService _tokenService, IMapper _mapper)
    {
        context = _context;
        tokenService = _tokenService;
        mapper = _mapper;
    }

    [HttpPost("register")] // account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExist(registerDto.Username))
        {
            return BadRequest("Username is already taken!");
        }

        using var hmac = new HMACSHA512();

        var user = mapper.Map<AppUser>(registerDto);

        user.UserName = registerDto.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        user.PasswordSalt = hmac.Key;

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.Users
        .Include(p => p.Photos)
            .FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

        if (user == null)
        {
            return Unauthorized("Invalid username!");
        }

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i])
            {
                return Unauthorized("Invalid password!");
            }
        }

        return new UserDto {
            Username = user.UserName,
            KnownAs = user.KnownAs,
            Token = tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            Gender = user.Gender
        };
    }

    private async Task<bool> UserExist(string username)
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower()); // Bob != bob
    }
}
