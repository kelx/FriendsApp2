using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FriendsApp2.Api.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
namespace FriendsApp2.Api.helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var repo = resultContext.HttpContext.RequestServices.GetService<IFriendsRepository>();
            var user = await repo.GetUser(userId, true);
            user.LastActive = DateTime.Now;
            repo.Update(user);
            await repo.SaveAll();
        }
    }
}