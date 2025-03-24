using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI_PhanTranMinhTam_New.Authorization
{

    public class PermisstionAttribute : TypeFilterAttribute
    {
        public PermisstionAttribute(string permissionName) : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { new PermissionRequirement(permissionName) };
        }
    }
    public class PermissionFilter : IAsyncAuthorizationFilter
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly PermissionRequirement _requirement;

        public PermissionFilter(IAuthorizationService authorizationService, PermissionRequirement requirement)
        {
            _authorizationService = authorizationService;
            _requirement = requirement;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            AuthorizationResult result = await _authorizationService.AuthorizeAsync(context.HttpContext.User, null, _requirement);

            if (!result.Succeeded)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
