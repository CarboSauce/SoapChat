using CoreWCF;
using LiteDB;
using Microsoft.AspNetCore.Identity;

namespace SoapChat.DbServer.Features;

[ServiceContract]
public interface IUserService
{
    [OperationContract]
    UserResponse CreateUser(string name, string password);

    [OperationContract]
    UserResponse GetUser(string id);

    [OperationContract]
    UserResponse[] SearchUsers(string nameSearch);

    [OperationContract]
    UserResponse ValidateCredentials(string username, string password);

    [OperationContract]
    void UploadAvatar(string userId, byte[] data);

    [OperationContract]
    byte[] GetAvatar(string userId);
}

public class UserService(LiteDbContext dbContext, IPasswordHasher<User> hasher)
    : IUserService
{
    private readonly LiteDatabase context = dbContext.Context;

    public UserResponse CreateUser(string name, string password)
    {
        var col = context.GetCollection<User>(SchemaNames.Users);
        col.EnsureIndex(x => x.Name, true);
        var user = new User { Name = name };
        user.PasswordHash = hasher.HashPassword(user, password);

        col.Insert(user);

        return user.ToResponse();
    }

    public UserResponse GetUser(string id)
    {
        var userId = new ObjectId(id);
        var user = context
            .GetCollection<User>(SchemaNames.Users)
            .FindById(userId);
        return user?.ToResponse()
            ?? throw new KeyNotFoundException("User not found");
    }

    public UserResponse[] SearchUsers(string nameSearch)
    {
        var col = context.GetCollection<User>(SchemaNames.Users);
        nameSearch = nameSearch.ToUpper();
        var users = col.Find(x => x.Name.ToUpper() == nameSearch).ToArray();

        return users.Select(x => x.ToResponse()).ToArray();
    }

    public UserResponse ValidateCredentials(string username, string password)
    {
        var col = context.GetCollection<User>(SchemaNames.Users);
        col.EnsureIndex(x => x.Name, true);
        var user = col.FindOne(x => x.Name == username);

        if (user == null)
            throw new UnauthorizedAccessException("Credentials invalid");

        var result = hasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            password
        );
        return result == PasswordVerificationResult.Success
            ? user.ToResponse()
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
