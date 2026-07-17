namespace AdGuardHomeGUI.Interfaces;

public interface IAdGuardApiService
{
    Task<bool> GetProtectionStatusAsync();
    Task SetProtectionAsync(bool enabled);
}