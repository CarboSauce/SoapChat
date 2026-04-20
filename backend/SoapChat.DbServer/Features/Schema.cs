using LiteDB;

namespace SoapChat.DbServer.Features;

public static class SchemaNames
{
    public const string Users = "Users";
    public const string Groups = "Groups";
    public const string Messages = "Messages";
}

public class User
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string PasswordHash { get; set; }
}

public class ChatGroup
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string Name { get; set; }

    [BsonRef(SchemaNames.Users)]
    public List<User> Members { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ChatMessage
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonRef(SchemaNames.Groups)]
    public ChatGroup Group { get; set; }

    [BsonRef(SchemaNames.Users)]
    public User Sender { get; set; }

    public string Text { get; set; }

    public DateTime SentAt { get; set; }
}
