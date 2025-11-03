using Demo.Invoices.API.Hosting.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Net.WebSockets;

namespace Demo.Invoices.API;

/// <remarks>
/// Each method (AddApplication, AddInfrastructure, AddHosting) is responsible for registering dependencies related to its respective layer.
/// Usually, these methods would be placed in their respective projects (e.g., Application, Infrastructure, Hosting).
/// </remarks>
public static class Registration
{
    /// <summary>
    /// Adds services required by the Application layer (business logic). Usually, these code reside in the Application project.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var invoiceServiceConfiguration = configuration.GetSection("InvoiceServiceConfiguration").Get<InvoiceServiceConfiguration>();

        if (invoiceServiceConfiguration == null)
        {
            throw new Exception("InvoiceServiceConfiguration is not configured properly.");
        }
        services.AddSingleton<InvoiceServiceConfiguration>(invoiceServiceConfiguration);
        services.AddScoped<IInvoiceService, InvoiceService>();

        return services;
    }

    /// <summary>
    /// Adds services required by the Infrastructure layer (data access, external API clients, etc.). Usually, these code reside in the Infrastructure project.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<ICurrencyApiClient, CurrencyApiClient>();
        services.AddScoped<ICustomerApiClient, CustomerApiClient>();

        var dbContextOptions = InvoiceDbContextOptionsBuilder.Create(configuration);
        services.AddSingleton(dbContextOptions);
        services.AddScoped<InvoiceDbContext>();

        return services;
    }

    /// <summary>
    /// Adds services required by the Hosting layer (web hosting, authentication, authorization, etc.). Usually, these code reside in the Hosting project.
    /// </summary>
    public static IServiceCollection AddHosting(this IServiceCollection services, IConfiguration configuration)
    {
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
        services.AddSwaggerGen();
        services.AddControllers();

        services.AddScoped<ContextMiddleware.ExecutionContext>();
        services.AddScoped<IExecutionContext>(provider => provider.GetRequiredService<ContextMiddleware.ExecutionContext>());

        services.AddScoped<UserContext>();
        services.AddScoped<IUserContext, UserContext>(provider => provider.GetRequiredService<UserContext>());

        services.AddHostingSecurity(configuration);

        return services;
    }

    /// <summary>
    /// Moved here to keep hosting related security code isolated.
    /// </summary>
    public static IServiceCollection AddHostingSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<FakeUserStore>();

        services.AddSingleton<IAuthorizationHandler,PermissionAuthorizationHandler>();

        var jwtTokenSymmetricSigningCredentials = new JwtTokenSymmetricSigningCredentials(configuration);
        services.AddSingleton(jwtTokenSymmetricSigningCredentials);


        services.AddAuthentication("Basic")
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", options => { });

        services.AddAuthentication().AddJwtBearer("BearerSymetric", options =>
        {
            options.Authority = jwtTokenSymmetricSigningCredentials.Issuer;
            options.Audience = jwtTokenSymmetricSigningCredentials.Audience;
            options.RequireHttpsMetadata = false;
            options.MapInboundClaims = false;

            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtTokenSymmetricSigningCredentials.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtTokenSymmetricSigningCredentials.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = jwtTokenSymmetricSigningCredentials.SymmetricSecurityKey,
                ValidateLifetime = true,
                RoleClaimType = DemoClaimTypes.Role //Ensure that authentication handler maps roles to standardized claim type without us having to implicitly do it in ClaimsTransformation step
            };
        });

        services.AddAuthentication().AddJwtBearer("BearerAsymetric", options =>
        {
            options.Authority = "https://sts.windows.net/ff7e6dff-bfb1-4975-81af-ed5c8bb2d406/";
            options.Audience = "api://anonymous-api/dev";
        });

        //This will fire if no authorization attribute is found on the endpoint
        AuthorizationPolicyBuilder fallbackPolicyBuilder = new AuthorizationPolicyBuilder();
        fallbackPolicyBuilder.AddAuthenticationSchemes(SecurityAuthenticationSchemes.All);
        fallbackPolicyBuilder.RequireAuthenticatedUser();
        var fallbackPolicy = fallbackPolicyBuilder.Build();

        //This will fire if default authorization attribute is found on the endpoint    
        AuthorizationPolicyBuilder defaultPolicyBuilder = new AuthorizationPolicyBuilder();
        defaultPolicyBuilder.AddAuthenticationSchemes(SecurityAuthenticationSchemes.All);
        defaultPolicyBuilder.RequireAuthenticatedUser();
        defaultPolicyBuilder.RequireAssertion(ctx =>
        {
            // We not only want to receive valid token but also it has to contain base identification claims e.g, email
            return UserContext.CanBeAuthenticated(ctx.User);
        });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = fallbackPolicyBuilder.Build();
            options.DefaultPolicy = defaultPolicyBuilder.Build();

            options.AddPolicy("dynamicDevPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                foreach (var schema in SecurityAuthenticationSchemes.All)
                {
                    policy.AuthenticationSchemes.Add(schema);
                }
                policy.RequireAssertion(context =>
                {
                    var isAuth = context.User.Claims.Any(c => c.Type.Equals("roles") && c.Value.Contains("Admin"));
                    return isAuth;
                });
            });
        });

        return services;
    }



    /// <summary>
    /// Configures the HTTP request pipeline. Usually, this code reside in the Hosting project. 
    /// Not always we want to add HTTP request pipeline. If designed properly our app could be compiled to single executable and run in two modes:
    /// * API - with HTTP request pipeline and without background processing
    /// * Worker - without HTTP request pipeline and with background processing
    /// Allowing to properly scale each mode independently.
    /// </summary>
    public static async Task ConfigureHostingPipeline(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            ///openapi/v1.json
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<ContextMiddleware>();

        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var invoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
            await invoiceRepository.Initialize(app.Lifetime.ApplicationStopping);
        }
    }
}