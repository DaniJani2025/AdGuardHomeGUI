using AdGuardHomeGUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.Json;

namespace AdGuardHomeGUI.Services
{
    public class SessionStorageService
    {
        public SessionStorageService()
        {
            _file = Path.Combine(_folder, "session.dat");
        }

        public void Save(SessionCookie cookie)
        {
            Directory.CreateDirectory(_folder);

            string json = JsonSerializer.Serialize(cookie);

            byte[] plainBytes = Encoding.UTF8.GetBytes(json);

            byte[] encrypted =
                ProtectedData.Protect(
                    plainBytes,
                    null,
                    DataProtectionScope.CurrentUser);

            File.WriteAllBytes(_file, encrypted);
        }

        public SessionCookie? Load()
        {
            if (!File.Exists(_file))
                return null;

            byte[] encrypted = File.ReadAllBytes(_file);

            byte[] plain =
                ProtectedData.Unprotect(
                    encrypted,
                    null,
                    DataProtectionScope.CurrentUser);

            string json = Encoding.UTF8.GetString(plain);

            return JsonSerializer.Deserialize<SessionCookie>(json);
        }

        public void Delete()
        {
            if (File.Exists(_file))
                File.Delete(_file);
        }

        private readonly string _folder =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "AdGuardHomeGUI");

        private readonly string _file;
    }
}
