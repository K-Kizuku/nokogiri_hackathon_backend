using System;
namespace nokogiri_hackathon_backend.Models;

//[Immutable]
[GenerateSerializer]
public record class ChatItem(
    string? Name,
    string Message)
{
    [Id(0)]
    public string Name { get; init; } = Name ?? "Alexey";

    [Id(1)]
    public DateTimeOffset Created { get; init; } = DateTimeOffset.Now;
};

