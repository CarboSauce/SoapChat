namespace SoapChat.Api.Features.Mutation;

[MutationType]
public static class Account
{
    public static bool Login(string username, string password)
    {
        return true;
    }
}
