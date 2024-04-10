using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServerApi.DTOs;
using WorldCitiesModel;

namespace ServerApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController(UserManager<WorldCitiesUser> userManager, JwtHandler jwtHandler) : ControllerBase
{
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        WorldCitiesUser? user = await userManager.FindByNameAsync(loginRequest.UserName);
        if (user == null)
        {
            return Unauthorized("Bad user name");
        }

        bool success = await userManager.CheckPasswordAsync(user, loginRequest.Password);
        if (!success)
        {
            return Unauthorized("Wrong password");
        }

        JwtSecurityToken secToken = await jwtHandler.GetTokenAsync(user);
        string? jwtstr = new JwtSecurityTokenHandler().WriteToken(secToken);
        return Ok(new LoginResult
        {
            Success = true,
            Message = "Mom loves me",
            Token = jwtstr
        });
    }
}