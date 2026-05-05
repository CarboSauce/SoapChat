using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using SoapChat.Api.ChatGroupService;
using SoapChat.Api.MessageService;
using SoapChat.Api.Services;
using SoapChat.Api.UserService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddServiceDefaults();

// Configure JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey =
    jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException("JWT SecretKey not configured");
var issuer =
    jwtSettings["Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer not configured");
var audience =
    jwtSettings["Audience"]
    ?? throw new InvalidOperationException("JWT Audience not configured");
var corsUrl =
    builder.Configuration["CorsUrl"]
    ?? throw new InvalidOperationException("CORS URL not configured");
var expirationMinutesStr = jwtSettings["ExpirationMinutes"] ?? "1440";
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy
            .WithOrigins($"http://{corsUrl}", $"https://{corsUrl}")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

builder
    .Services.AddAuthentication("Bearer")
    .AddJwtBearer(
        "Bearer",
        options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true
            };
        }
    )
    .AddCookie(
        CookieAuthenticationDefaults.AuthenticationScheme,
        options => builder.Configuration.Bind("CookieSettings", options)
    );

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton(
    new JwtTokenGenerator(
        secretKey,
        issuer,
        audience,
        int.Parse(expirationMinutesStr)
    )
);

builder.Services.AddGraphQLServer().AddApiTypes();

builder.Services.AddScoped<ChatGroupServiceClient>(provider =>
{
    var url = builder.Configuration["DbServerUrl"];
    return new ChatGroupServiceClient(
        ChatGroupServiceClient
            .EndpointConfiguration
            .BasicHttpBinding_IChatGroupService1,
        $"{url}/ChatGroup.asmx"
    );
});

builder.Services.AddScoped<UserServiceClient>(provider =>
{
    var url = builder.Configuration["DbServerUrl"];
    return new UserServiceClient(
        UserServiceClient.EndpointConfiguration.BasicHttpBinding_IUserService1,
        $"{url}/User.asmx"
    );
});

builder.Services.AddScoped<MessageServiceClient>(provider =>
{
    var url = builder.Configuration["DbServerUrl"];
    return new MessageServiceClient(
        MessageServiceClient
            .EndpointConfiguration
            .BasicHttpBinding_IMessageService1,
        $"{url}/Message.asmx"
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

app.Use(
    async (context, next) =>
    {
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            var token = context.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Request.Headers["Authorization"] = "Bearer " + token;
            }
        }

        await next();
    }
);

app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();
app.UseHttpsRedirection();

await app.RunAsync();
