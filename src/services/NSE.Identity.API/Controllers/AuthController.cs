using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSE.Identity.API.Extensions;
using NSE.Identity.API.Models;

namespace NSE.Identity.API.Controllers;

[ApiController]
[Route("api/identity")]
public class AuthController : MainController
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AppSettings _appSettings;

    public AuthController(SignInManager<IdentityUser> signInManager, 
        UserManager<IdentityUser> userManager, IOptions<AppSettings> appSettings)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _appSettings = appSettings.Value;
    }
    
    [HttpPost("new-account")]
    public async Task<ActionResult> Register(NewUser newUser)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var user = new IdentityUser
        {
            UserName = newUser.Email,
            Email = newUser.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, newUser.Password);

        if (result.Succeeded)
        {
            return CustomResponse(await GenerateJwt(newUser.Email));
        }

        foreach (var error in result.Errors)
        {
            AddErrorToStack(error.Description);
        }
        
        return CustomResponse();
    }
    
    [HttpPost("authenticate")]
    public async Task<ActionResult> Login(UserLogin userLogin)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var result = await _signInManager.PasswordSignInAsync(userLogin.Email, userLogin.Password, false, true);

        if (result.Succeeded)
        {
            return Ok(await GenerateJwt(userLogin.Email));
        }

        if (result.IsLockedOut)
        {
            AddErrorToStack("User temporary blocked. Too many tries.");
            return CustomResponse();
        }
        
        AddErrorToStack("User or Password incorrect");
        return CustomResponse();
    }

    private async Task<UserLoginResponse> GenerateJwt(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var claims = await _userManager.GetClaimsAsync(user!);

        var identityClaims = await GetClaimsUser(claims, user!);
        var encodedToken = WriteToken(identityClaims);

        return GetTokenResponse(encodedToken, user!, claims);
    }

    private async Task<IEnumerable<Claim>> GetClaimsUser(ICollection<Claim> claims, IdentityUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user!);
        
        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user!.Id));
        claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email!));
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

        foreach (var userRole in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        return claims;
    }
    
    private string WriteToken(IEnumerable<Claim> identityClaims)
    {
        var key = Encoding.ASCII.GetBytes(_appSettings.Key);

        var tokenConfig = new JwtSecurityToken(issuer: _appSettings.Issuer,
            audience: _appSettings.Audience,
            claims: identityClaims,
            expires: DateTime.UtcNow.AddHours(_appSettings.Expires),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature));

        return new JwtSecurityTokenHandler().WriteToken(tokenConfig);
    }
    
    private UserLoginResponse GetTokenResponse(string encodedToken, IdentityUser user, IList<Claim> claims)
    {
        return new UserLoginResponse
        {
            AccessToken = encodedToken,
            ExpiresIn = TimeSpan.FromHours(_appSettings.Expires).TotalSeconds,
            UserToken = new UserToken
            {
                Id = user.Id,
                Email = user.Email!,
                Claims = claims.Select(s => new UserClaim { Type = s.Type, Value = s.Value })
            }
        };
    }
}