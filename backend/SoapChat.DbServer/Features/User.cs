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

    [OperationContract]
    void UploadAvatar(string userId, byte[] data);

    [OperationContract]
    byte[] GetAvatar(string userId);
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
        nameSearch = nameSearch.ToUpper();
        var users = col.Find(x => x.Name.ToUpper() == nameSearch).ToArray();

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

    public void UploadAvatar(string userId, byte[] data)
    {
        var col = context.GetCollection<User>(SchemaNames.Users);
        var uid = new ObjectId(userId);
        var user = col.FindById(uid);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        var fs = context.FileStorage;
        using var stream = new MemoryStream(data);
        fs.Upload(userId, userId, stream);
    }

    public byte[] GetAvatar(string userId)
    {
        var col = context.GetCollection<User>(SchemaNames.Users);
        var uid = new ObjectId(userId);
        var user = col.FindById(uid);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        var fs = context.FileStorage;
        if (!fs.Exists(userId))
            throw new FileNotFoundException("Avatar not found");

        using var stream = fs.OpenRead(userId);
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}
