using AuthTest.Filter;
using AuthTest.JWTToken;
using AuthTest.Request;
using AuthTest.Request.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using System.Xml.Linq;

namespace AuthTest.Controllers;

//[ServiceFilter<AuthLogOutFilter>]
[Route("api/[controller]/[action]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly JwtHelpers _jwtHelpers;
    private readonly IMemoryCache _memoryCache;

    public LoginController(JwtHelpers jwtHelpers, IMemoryCache memoryCache)
    {
        _jwtHelpers = jwtHelpers;
        _memoryCache = memoryCache;
    }

    [AllowAnonymous]
    [HttpPost]
    public IResult PostTest([FromBody] LoginDTO loginDTO)
    {
        return Results.Ok(loginDTO);
    }

    [HttpPost]
    public IResult PostTestWithAuth([FromBody] LoginDTO loginDTO)
    {
        return Results.Ok(loginDTO);
    }

    //-----------------------

    [HttpGet]
    public IResult LogOut()
    {
        // 獲取 jwt 內多久後過期(毫秒)
        string? jwt_expireTimeString = User.Claims.FirstOrDefault(p => p.Type == "exp")?.Value;
        long jwt_expireTime = Convert.ToInt64(jwt_expireTimeString);

        DateTime start = DateTime.UtcNow;
        DateTime exp = start.AddMilliseconds(jwt_expireTime).ToLocalTime();

        DateTimeOffset offset = new DateTimeOffset(exp, new TimeSpan(8, 0, 0));

        string? jwt_id = User.Claims.FirstOrDefault(p => p.Type == "jti")?.Value;

        var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(offset);
        _memoryCache.Set(jwt_id, true, cacheEntryOptions);

        return Results.Ok(new { message = "你已登出" });
    }

    [HttpGet]
    [Authorize(Roles = "Cat,Boss,Manager,Employee")]
    public IResult GetIsCat()
    {
        return Results.Ok(new { message = "you are cat" });
    }

    [HttpGet]
    public IResult GetClaims()
    {
        var user = Request.HttpContext.User.Claims.Select(p => new { p.Type, p.Value });
        var jsonstring = Newtonsoft.Json.JsonConvert.SerializeObject(user);
        Console.WriteLine(jsonstring);
        return Results.Ok(user);
    }

    [HttpGet]
    public IResult GetUsername()
    {
        return Results.Ok(User.Identity?.Name);
    }

    [HttpGet]
    public IResult GetIsInRole(string roleName)
    {
        return Results.Ok(User.IsInRole(roleName));
    }

    [HttpGet]
    public IResult GetJwtId()
    {
        return Results.Ok(User.Claims.FirstOrDefault(p => p.Type == "jti")?.Value);
    }

    /// <summary>
    /// 登入並發出jwt
    /// </summary>
    /// <param name="loginDTO">帳號及密碼</param>
    /// <returns>jwt token</returns>
    [AllowAnonymous]
    [HttpPost]
    public IResult SignIn(LoginDTO loginDTO)
    {
        if (ValidateUser(loginDTO))
        {
            var token = _jwtHelpers.GenerateToken(loginDTO.Username);
            return Results.Ok(token);
        }
        return Results.Ok(new { message = "帳號或密碼錯誤" });
    }

    bool ValidateUser(LoginDTO login)
    {
        if (login.Username != "string") return false;
        if (login.Password != "string") return false;

        return true;
    }
}

