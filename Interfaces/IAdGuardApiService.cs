namespace AdGuardHomeGUI.Interfaces;

public interface IAdGuardApiService
{
    Task<bool> IsProtectionEnabled();
    Task SetProtection(bool enabled);
}