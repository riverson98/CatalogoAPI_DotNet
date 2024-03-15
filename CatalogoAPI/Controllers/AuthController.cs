using Asp.Versioning;
using CatalogoAPI.DTOs;
using CatalogoAPI.Models;
using CatalogoAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace CatalogoAPI.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ITokenService tokenService, UserManager<ApplicationUser> userManager,
                            RoleManager<IdentityRole> roleManager, IConfiguration config, ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _config = config;
        _logger = logger;
    }

    [HttpPost]
    [Route("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.Username!);

        if(user is not null && await _userManager.CheckPasswordAsync(user, loginDto.Password!))
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("id", user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = _tokenService.GenerateAcessToken(authClaims, _config);
            var refreshToken = _tokenService.GenerateRefreshToken();
            _ = int.TryParse(_config["JWT:RefreshTokenValidityInMinutes"],
                             out int refreshTokenValidityInMinutes);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);
            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
        }
        return Unauthorized();
    }

    [HttpPost]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        var userExist = await _userManager.FindByNameAsync(dto.Username!);
        if (userExist is not null)
            return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response {Status = "Error", Message = "User already exists!"});

        ApplicationUser user = new()
        {
            Email = dto.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = dto.Username
        };
        var result = await _userManager.CreateAsync(user, dto.Password!);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User creation failed." });

        return Ok(new Response { Status = "Success", Message = "User created successfully!"});
    }

    [HttpPost]
    [Route("refresh-token")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken(TokenDTO tokenDto)
    {
        if (tokenDto is null)
            return BadRequest("Invalid client request");

        string? accessToken = tokenDto.AcessToken
                                ?? throw new ArgumentNullException(nameof(tokenDto));
        string? refreshToken = tokenDto.RefreshToken 
                                ?? throw new ArgumentNullException( nameof(tokenDto));
        var principal = _tokenService.GetClaimsPrincipal(accessToken, _config);

        if (principal is null)
            return BadRequest("Invalid access token/refresh token");

        var username = principal.Identity?.Name;
        var user = await _userManager.FindByNameAsync(username!);

        if (user is null ||!user.RefreshToken!.Equals(refreshToken)
                         || user.RefreshTokenExpiryTime <= DateTime.Now)
            return BadRequest("Invalid access token/refresh token");

        var newAcessToken = _tokenService.GenerateAcessToken(principal.Claims.ToList(), _config);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);
        
        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAcessToken),
            refreshToken = newRefreshToken,
        });
    }

    [Authorize(AuthenticationSchemes = "Bearer", Policy = "ExclusiveOnly")]
    [HttpPost]
    [Route("revoke/{username}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Revoke(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        
        if (user is null) 
            return BadRequest("Invalid user name");
        
        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);
        
        return NoContent();
    }

    [HttpPost]
    [Route("createRole")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "SuperAdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var roleExist = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (roleResult.Succeeded)
            {
                _logger.LogInformation(1, "Roles added");
                return StatusCode(StatusCodes.Status200OK,
                        new Response { Status = "Success", Message = $"Role {roleName} added successfully" });
            }
            else
            {
                _logger.LogError(2, "Error");
                return StatusCode(StatusCodes.Status400BadRequest,
                        new Response { Status = "Error", Message = $"Issue adding the new {roleName} role" });
            }
        }
        _logger.LogInformation(3, "Role already exist");
        return StatusCode(StatusCodes.Status400BadRequest,
                        new Response { Status = "Error", Message = $"Role already exist." });
    }

    [HttpPost]
    [Route("AddUserToRole")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "SuperAdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if(user is not null)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                _logger.LogInformation(1, $"User {user.Email} added to the {roleName} role");
                return StatusCode(StatusCodes.Status200OK,
                        new Response { Status = "Success", Message = $"User {user.Email} added to the {roleName} role" });
            }
            else
            {
                _logger.LogError(1, $"Error: unable to add user {user.Email} to the {roleName} role");
                return StatusCode(StatusCodes.Status400BadRequest,
                        new Response { 
                                Status = "Error", Message = $"Error: unable to add user {user.Email} to the {roleName} role" 
                        });
            }
        }
        return BadRequest(new { error = $"Unable to find user {email}" });
    }
}
