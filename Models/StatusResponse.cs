using System.Text.Json.Serialization;

namespace AdGuardHomeGUI.Models;

public class StatusResponse
{
    [JsonPropertyName("protection_enabled")]
    public bool ProtectionEnabled { get; set; }
}