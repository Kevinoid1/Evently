using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Evently.Common.Infrastructure.Authorization;

internal sealed class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _authorizationOptions;
    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
        _authorizationOptions = options.Value;
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
       AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);
       if (policy is not null)
       {
           return policy;
       }
       
       var permissions = policyName.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();

       AuthorizationPolicy permissionPolicy = new AuthorizationPolicyBuilder()
           .AddRequirements(new PermissionRequirement(permissions)).Build();
       
       _authorizationOptions.AddPolicy(policyName, permissionPolicy);

       return permissionPolicy;
    }
}
