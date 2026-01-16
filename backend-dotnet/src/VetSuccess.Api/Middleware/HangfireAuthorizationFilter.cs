using Hangfire.Dashboard;

namespace VetSuccess.Api.Middleware;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        
        // In development, allow all access
        if (httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            return true;
        }
        
        // In production, require authentication
        return httpContext.User.Identity?.IsAuthenticated ?? false;
    }
}
