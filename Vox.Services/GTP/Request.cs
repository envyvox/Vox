using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Vox.Services.GTP;

public class Request
{
    [JsonPropertyName("model")] public string ModelId { get; set; } = "";
    [JsonPropertyName("messages")] public List<Message> Messages { get; set; } = new();
}