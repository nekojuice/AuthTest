using AuthTest.JWTToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace AuthTest.Filter
{
    public class AuthLogOutFilter : IAuthorizationFilter
    {
        private readonly IMemoryCache _memoryCache;

        public AuthLogOutFilter(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var jwt_id = context.HttpContext.User.Claims.FirstOrDefault(p => p.Type == "jti")?.Value;
            bool inBlacklist = false;
            if (!string.IsNullOrEmpty(jwt_id))
            {
                inBlacklist = _memoryCache.Get<bool>(jwt_id);
            }

            if (inBlacklist) 
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
