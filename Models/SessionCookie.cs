namespace AdGuardHomeGUI.Models;

public class SessionCookie
{
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public string Domain { get; set; } = "";
    public string Path { get; set; } = "/";
    public DateTime? Expires { get; set; }
}