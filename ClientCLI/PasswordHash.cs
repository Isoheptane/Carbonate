using System;
using System.Text;
using System.Security.Cryptography;

namespace ClientCLI
{
    public static class PasswordHash
    {
        public static string SHA256x7(string password)
        {
            SHA256 sha256 = SHA256.Create();
            sha256.Initialize();
            byte[] data = Encoding.UTF8.GetBytes(password);
            for (int count = 0; count < 7; count++)
                data = sha256.ComputeHash(data);
            StringBuilder hashString = new StringBuilder();
            foreach (byte oct in data)
                hashString.Append(oct.ToString("X"));
            return hashString.ToString();
        }
    }
}
