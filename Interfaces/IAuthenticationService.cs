using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AdGuardHomeGUI.Interfaces
{
    public interface IAuthenticationService
    {
        HttpClient HttpClient { get; }

        bool IsAuthenticated { get; }

        Task<bool> LoginAsync(string username, string password, bool rememberMe);
        Task<bool> TryRestoreSessionAsync();
        Task LogoutAsync();
    }
}
