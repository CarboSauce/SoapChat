using CoreWCF;
using LiteDB;
using Microsoft.AspNetCore.Identity;

namespace SoapChat.DbServer.Features;

[ServiceContract]
public interface IUserService
{
    [OperationContract]
    User CreateUser(string name, string password);

    [OperationContract]
    User GetUser(string id);

    [OperationContract]
    User[] SearchUsers(string nameSearch);

    [OperationContract]
    User ValidateCredentials(string username, string password);
}

public class UserService(LiteDbContext dbContext, IPasswordHasher<User> hasher)
    : IUserService
{
    private readonly LiteDatabase context = dbContext.Context;

    public User CreateUser(string name, string password)
    {
        var col = context.GetCollection<User>(SchemaNames.Users);
        col.EnsureIndex(x => x.Name, true);
        var user = new User { Name = name };
        user.PasswordHash = hasher.HashPassword(user, password);

        col.Insert(user);

        return user;
    }

    public User GetUser(string id)
    {
        var userId = new ObjectId(id);
        return context.GetCollection<User>(SchemaNames.Users).FindById(userId);
    }

    public User[] SearchUsers(string nameSearch)
    {
        var col = context.GetCollection<User>(SchemaNames.Users);
        var users = col.Find(x =>
                x.Name.Contains(nameSearch, StringComparison.OrdinalIgnoreCase)
            )
            .ToArray();

        return users;
    }

    public User ValidateCredentials(string username, string password)
    {
        var col = context.GetCollection<User>(SchemaNames.Users);
        col.EnsureIndex(x => x.Name, true);
        var user = col.FindOne(x => x.Name == username);
        var result = hasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            password
        );
        return result == PasswordVerificationResult.Success
            ? user
            : throw new UnauthorizedAccessException("Credentials invalid");
    }
}
