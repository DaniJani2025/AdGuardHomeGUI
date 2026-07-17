using System.Text.Json.Serialization;

namespace AdGuardHomeGUI.Models;

public class ProtectionRequest
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("duration")]
    public int? Duration { get; set; }
}