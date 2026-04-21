namespace SoapChat.DbServer.Features;

public class UserResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class ChatGroupResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public UserResponse[] Members { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

public class ChatMessageResponse
{
    public string Id { get; set; }
    public ChatGroupResponse Group { get; set; }
    public UserResponse Sender { get; set; }
    public string Text { get; set; }
    public DateTime SentAt { get; set; }
}

public static class ResponseMapper
{
    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse { Id = user.Id.ToString(), Name = user.Name };
    }

    public static ChatGroupResponse ToResponse(this ChatGroup group)
    {
        return new ChatGroupResponse
        {
            Id = group.Id.ToString(),
            Name = group.Name,
            Members = group.Members.Select(x => x.ToResponse()).ToArray(),
            CreatedAt = group.CreatedAt
        };
    }

    public static ChatMessageResponse ToResponse(this ChatMessage message)
    {
        return new ChatMessageResponse
        {
            Id = message.Id.ToString(),
            Group = message.Group.ToResponse(),
            Sender = message.Sender.ToResponse(),
            Text = message.Text,
            SentAt = message.SentAt
        };
    }
}
