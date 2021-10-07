using System;
using System.Threading.Tasks;
using API.Extentions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultcontext = await next();

            if(!resultcontext.HttpContext.User.Identity.IsAuthenticated) return;

            var unitOfWork = resultcontext.HttpContext.RequestServices.GetService<IUnitOfWork>();
            var user = await unitOfWork.UserRepository.GetUserByIDAsync(resultcontext.HttpContext.User.getUserId());
            user.LastActive = DateTime.UtcNow;
            await unitOfWork.Complete();
        }
    }
} 