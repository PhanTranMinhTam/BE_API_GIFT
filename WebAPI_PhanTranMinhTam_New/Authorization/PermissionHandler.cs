using Microsoft.AspNetCore.Authorization;


namespace WebAPI_PhanTranMinhTam_New.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            bool hasClaim = context.User.Claims.Any(c => c.Type == "Permission" && c.Value == requirement.PermissionName);

            if (hasClaim)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
