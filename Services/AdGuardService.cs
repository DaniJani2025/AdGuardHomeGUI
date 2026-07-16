using System.ServiceProcess;

namespace AdGuardHomeGUI.Services;

public class AdGuardService
{
    private const string ServiceName = "AdGuardHome";

    public bool IsRunning()
    {
        using var service = new ServiceController(ServiceName);

        return service.Status == ServiceControllerStatus.Running;
    }

    public ServiceControllerStatus GetStatus()
    {
        using var service = new ServiceController(ServiceName);

        return service.Status;
    }

    public void Start()
    {
        using var service = new ServiceController(ServiceName);

        if (service.Status == ServiceControllerStatus.Running)
            return;

        service.Start();
        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
    }

    public void Stop()
    {
        using var service = new ServiceController(ServiceName);

        if (service.Status == ServiceControllerStatus.Stopped)
            return;

        service.Stop();
        service.Refresh();

        if (service.Status != ServiceControllerStatus.Stopped)
        {
            service.WaitForStatus(
                ServiceControllerStatus.Stopped,
                TimeSpan.FromSeconds(30));
        }
    }

    public void Restart()
    {
        Stop();
        Start();
    }
}