using System;
namespace nokogiri_hackathon_backend.Models;
internal readonly record struct ClientContext(
    IClusterClient Client,
    string? UserName = null,
    string? CurrentChannel = null);

