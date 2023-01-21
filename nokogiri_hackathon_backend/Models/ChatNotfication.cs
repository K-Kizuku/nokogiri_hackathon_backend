namespace nokogiri_hackathon_backend.Models;

[Immutable]
[GenerateSerializer]
public record class TodoNotification(
    Guid ItemKey,
    ChatItem? Item = null);