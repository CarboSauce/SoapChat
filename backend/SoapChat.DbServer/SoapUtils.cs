using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Description;

namespace SoapChat.DbServer;

public static class SoapUtils
{
    public static void AddSoapService<Service, IService>(
        this WebApplication app,
        IServiceBuilder builder,
        string endpoint
    )
        where Service : class, IService
        where IService : class
    {
        builder.AddService<Service>();
        builder.AddServiceEndpoint<Service, IService>(
            new BasicHttpBinding(),
            endpoint
        );
        builder.AddServiceEndpoint<Service, IService>(
            new BasicHttpBinding(BasicHttpSecurityMode.Transport),
            endpoint
        );

        var serviceMetadataBehavior =
            app.Services.GetRequiredService<ServiceMetadataBehavior>();

        serviceMetadataBehavior.HttpGetEnabled = true;
        serviceMetadataBehavior.HttpsGetEnabled = true;
    }
}
