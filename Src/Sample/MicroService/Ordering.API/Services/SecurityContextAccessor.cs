using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ordering.API.Services
{
    public interface ISecurityContextAccessor
    {
        string UserId { get; }
        string Role { get; }
        string JwtToken { get; }
        bool IsAuthenticated { get; }

        string Phone { get; }
    }


    public class SecurityContextAccessor : ISecurityContextAccessor
    {
        private readonly ILogger<SecurityContextAccessor> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SecurityContextAccessor(IHttpContextAccessor httpContextAccessor,
            ILogger<SecurityContextAccessor> logger)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public string Phone
        {
            get
            {
                var phone = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.HomePhone)?.Value;
                return phone;
            }
        }
        public string UserId
        {
            get
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return userId;
            }
        }

        public string JwtToken
        {
            get
            {
                return _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"];
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                var isAuthenticated = _httpContextAccessor.HttpContext?.User?.Identities?.FirstOrDefault()?.IsAuthenticated;
                if (!isAuthenticated.HasValue)
                {
                    return false;
                }

                return isAuthenticated.Value;
            }
        }

        public string Role
        {
            get
            {
                var role = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
                return role;
            }
        }
    }
}
