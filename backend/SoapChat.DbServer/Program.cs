using CoreWCF.Configuration;
using CoreWCF.Description;
using Microsoft.AspNetCore.Identity;
using SoapChat.DbServer;
using SoapChat.DbServer.Features;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
ConfigureServices(builder.Services);
static void ConfigureServices(IServiceCollection services)
{
    services.AddServiceModelMetadata();
    services.AddServiceModelServices();
    services.AddSingleton<IServiceBehavior>(
        new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true }
    );
    // services.AddSingleton<
    //     IServiceBehavior,
    //     UseRequestHeadersForMetadataAddressBehavior
    // >();

    services.AddSingleton<LiteDbContext, LiteDbContext>();
    services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
    services.AddScoped<UserService>();
    services.AddScoped<MessageService>();
    services.AddScoped<ChatGroupService>();
}

var app = builder.Build();

app.UseHttpsRedirection();
app.UseServiceModel(builder =>
{
    app.AddSoapService<UserService, IUserService>(builder, "User.asmx");
    app.AddSoapService<MessageService, IMessageService>(
        builder,
        "Message.asmx"
    );
    app.AddSoapService<ChatGroupService, IChatGroupService>(
        builder,
        "ChatGroup.asmx"
    );
});

await app.RunAsync();
