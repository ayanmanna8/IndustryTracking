using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Services
{
    public class UserService : IUserService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserName()
        {
            var userName = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
            return userName ?? "unknown";
        }
    }

    public interface IUserService
    {
        string GetUserName();
    }
}
