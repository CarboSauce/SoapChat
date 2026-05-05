using LiteDB;
using Microsoft.Extensions.Options;

namespace SoapChat.DbServer;

public class LiteDbContext : IDisposable
{
    public LiteDatabase Context { get; }

    public LiteDbContext(IConfiguration config)
    {
        try
        {
            Context = new LiteDatabase(config.GetConnectionString("LiteDb"));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Failed to initialize LiteDB context.",
                ex
            );
        }
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}
