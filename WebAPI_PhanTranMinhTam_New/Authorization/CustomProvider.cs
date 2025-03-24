using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace WebAPI_PhanTranMinhTam_New.Authorization
{
    public class CustomProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;

        public CustomProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
            _options = options.Value;
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);
            if (policy != null)
            {
                return policy;
            }

            policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();

            return policy;
        }
    }
}
