using System.Text.Json;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Extensions
{
    public class CurrentUserServices 
    {
        public User? User { get; }
        public  CurrentUserServices(IHttpContextAccessor httpContextAccessor)
        {
            var stringUser = httpContextAccessor.HttpContext?.User?.FindFirst("UserData");
          
            if(stringUser == null)
            {
                return;
            }

            User = JsonSerializer.Deserialize<User>(stringUser.Value);
        }
    }
}