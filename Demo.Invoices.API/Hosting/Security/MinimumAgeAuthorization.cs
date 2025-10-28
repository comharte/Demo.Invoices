using Microsoft.AspNetCore.Authorization;

namespace Demo.Invoices.API.Hosting.Security;

class MinimumAgeAuthorizeAttribute : AuthorizeAttribute,
    IAuthorizationRequirement, IAuthorizationRequirementData
{
    public MinimumAgeAuthorizeAttribute(int age)
    {
        Age = age;
        AuthenticationSchemes = SecurityAuthenticationSchemes.AllConcatenated;
    }

    public int Age { get; set; }

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return this;
    }
}

class MinimumAgeAuthorizationHandler(ILogger<MinimumAgeAuthorizationHandler> logger)
    : AuthorizationHandler<MinimumAgeAuthorizeAttribute>
{
    // Check whether a given minimum age requirement is satisfied.
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MinimumAgeAuthorizeAttribute requirement)
    {
        logger.LogInformation(
            "Evaluating authorization requirement for age >= {age}",
            requirement.Age);

        // Get the user's birth date claim.
        var ageClaimValue =
            context.User.FindFirst(c => c.Type == "age")?.Value;

        if (ageClaimValue != null)
        {
            // If the user meets the age requirement, mark the authorization
            // requirement succeeded.
            if (!string.IsNullOrEmpty(ageClaimValue)
                && int.TryParse(ageClaimValue, out var age)
                && age >= requirement.Age)
            {
                logger.LogInformation(
                    "Minimum age authorization requirement {age} satisfied",
                    requirement.Age);
                context.Succeed(requirement);
            }
            else
            {
                logger.LogInformation(
                    "Current user's DateOfBirth claim ({dateOfBirth}) doesn't " +
                    "satisfy the minimum age authorization requirement {age}",
                    ageClaimValue,
                    requirement.Age);
            }
        }
        else
        {
            logger.LogInformation("No age claim present");
        }

        return Task.CompletedTask;
    }
}
