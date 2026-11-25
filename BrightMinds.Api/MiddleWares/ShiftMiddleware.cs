using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BrightMinds.Api.MiddleWares
{
    public class ShiftMiddleware
    {
        private readonly RequestDelegate _next;

        public ShiftMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext db)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(userId))
                {
                    var activeShift = await db.Shifts
                        .Where(s => s.UserId == userId && s.Status == ShiftStatus.Opened)
                        .OrderByDescending(s => s.StartTimeStamp)
                        .ThenByDescending(s => s.Id)
                        .FirstOrDefaultAsync();

                    if (activeShift != null)
                    {
                        // Inject into HttpContext — available everywhere downstream
                        context.Items["ActiveShift"] = activeShift;
                    }
                }
            }
            await _next(context);
        }
    }

    public static class ShiftMiddlewareExtensions
    {
        public static IApplicationBuilder UseShiftMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ShiftMiddleware>();
        }
    }
}
