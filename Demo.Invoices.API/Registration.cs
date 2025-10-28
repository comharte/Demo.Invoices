using Demo.Invoices.API.Hosting.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Demo.Invoices.API;

/// <remarks>
/// Each method (AddApplication, AddInfrastructure, AddHosting) is responsible for registering dependencies related to its respective layer.
/// Usually, these methods would be placed in their respective projects (e.g., Application, Infrastructure, Hosting).
/// </remarks>
public static class Registration
{
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

    public static IServiceCollection AddHosting(this IServiceCollection services, IConfiguration configuration)
    {
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
        services.AddSwaggerGen();
        services.AddControllers();

        var jwtTokenSymmetricSigningCredentials = new JwtTokenSymmetricSigningCredentials(configuration);
        services.AddSingleton(jwtTokenSymmetricSigningCredentials);
        services.AddSingleton<UserStore>();

        services.AddAuthentication("Basic")
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", options => { });

        services.AddScoped<IClaimsTransformation, ClaimsTransformation>();

        services.AddAuthentication().AddJwtBearer("BearerSymetric", options =>
        {
            options.Authority = jwtTokenSymmetricSigningCredentials.Issuer;
            options.Audience = jwtTokenSymmetricSigningCredentials.Audience;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtTokenSymmetricSigningCredentials.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtTokenSymmetricSigningCredentials.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = jwtTokenSymmetricSigningCredentials.SymmetricSecurityKey,
                ValidateLifetime = true,
            };
        });

        services.AddAuthentication().AddJwtBearer("BearerAsymetric", options =>
        {
            options.Authority = "https://sts.windows.net/ff7e6dff-bfb1-4975-81af-ed5c8bb2d406/";
            options.Audience = "api://anonymous-api/dev";
        });

        services.AddSingleton<IAuthorizationHandler,MinimumAgeAuthorizationHandler>();

        AuthorizationPolicyBuilder builder = new AuthorizationPolicyBuilder(["Basic", "BearerAsymetric"]);
        builder.RequireAssertion(context => context.User.Claims.Any(c => c.Type.Equals("master")));
        var masterPolicy = builder.Build();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("Dev", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AuthenticationSchemes.Add("Basic");
                policy.AuthenticationSchemes.Add("BearerSymetric");
                policy.RequireAssertion(context =>
                {
                    context.User.Claims.Any(c => c.Type.Equals("master") && c.Value.Equals("true"));
                    return true;
                });
            });
        });


        return services;
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
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

        app.MapControllers();

        return app;
    }
}