using System;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController : BaseApiController
{
    private readonly DataContext context;
    public BuggyController(DataContext _context)
    {
        context = _context;
    }

    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetAuth() 
    {
        return "secret text";
    }

    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound() 
    {
        var thing = context.Users.Find(-1);

        if (thing == null)
        {
            return NotFound();
        }

        return thing;
    }

    [HttpGet("server-error")]
    public ActionResult<AppUser> GetServerError() 
    {
        var thing = context.Users.Find(-1) ?? throw new Exception("A bad thing has happened");
        return thing;
    }

    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest() 
    {
        return BadRequest("This was not a good request");
    }

}
