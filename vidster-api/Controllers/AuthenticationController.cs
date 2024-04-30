using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vidster_api.Context;
using vidster_api.Models;
using vidster_api.ViewModels;

namespace vidster_api.Controllers;

[Route("api/authentication")]
[ApiController]
[Produces("application/json")]
[Authorize]
public class AuthenticationController(VidsterContext context) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("register"), AllowAnonymous]
    public async Task<IActionResult> Register(NewUserViewModel request)
    {
        if (context.Users.Any(u => u.Email == request.Email))
        {
            return Conflict("Email already registered");
        }

        CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordKey);

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Role = request.Role,
            PasswordHash = passwordHash,
            PasswordKey = passwordKey,
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        return StatusCode(201, "User was registered");
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("login"), AllowAnonymous]
    public async Task<IActionResult> Login(UserViewModel request)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordKey))
        {
            return Unauthorized("Wrong email or password");
        }

        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, user.Email),
            new(ClaimsIdentity.DefaultRoleClaimType, user.Role)
        };

        var id = new ClaimsIdentity(
            claims,
            "ApplicationCookie",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType
        );

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));

        return Ok(new
        {
            id = id,
            firstName = user.FirstName,
            lastName = user.LastName,
            avatar = user.Avatar,
            role = user.Role
        });
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("logout"), Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok("User was deauthorized");
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("me"), Authorize]
    public Task<IActionResult> GetMe()
    {
        var email = User.FindFirst(c => c.Type == ClaimsIdentity.DefaultNameClaimType)!.Value;
        var user = context.Users.First(u => u.Email == email);

        return Task.FromResult<IActionResult>(Ok(new
        {
            id = user.Id,
            firstName = user.FirstName,
            lastName = user.LastName,
            avatar = user.Avatar,
            role = user.Role
        }));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("check_authentication"), AllowAnonymous]
    public Task<IActionResult> CheckAuthentication()
    {
        return Task.FromResult<IActionResult>(
            Ok(User.FindFirst(c => c.Type == ClaimsIdentity.DefaultNameClaimType) != null));
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordKey)
    {
        using var hmac = new HMACSHA512();
        passwordKey = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordKey)
    {
        using var hmac = new HMACSHA512(passwordKey);
        var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computeHash.SequenceEqual(passwordHash);
    }
}