using Hangfire.Dashboard;

namespace HangfireAPI.Authorization;

public class AuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        return true;
    }
}