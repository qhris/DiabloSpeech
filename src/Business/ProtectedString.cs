// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System;
using System.Security.Cryptography;
using System.Text;

namespace DiabloSpeech.Business
{
    public static class ProtectedString
    {
        static readonly Encoding Encoding = Encoding.Unicode;
        static readonly DataProtectionScope ProtectionScope = DataProtectionScope.CurrentUser;

        public static string DecryptBase64(string encoded64)
        {
            if (string.IsNullOrEmpty(encoded64))
                return string.Empty;

            // Decode base64 string.
            byte[] encrypted = Convert.FromBase64String(encoded64);

            // Decrypt data back to string.
            byte[] decrypted = ProtectedData.Unprotect(encrypted, null, ProtectionScope);
            return Encoding.GetString(decrypted).TrimEnd();
        }

        public static string EncryptBase64(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException(nameof(data));
            data = data + new string(' ', data.Length % 8);

            // Encypt bytes for current user.
            byte[] buffer = Encoding.GetBytes(data);
            byte[] encrypted = ProtectedData.Protect(buffer, null, ProtectionScope);

            // Return safe value as base64.
            return Convert.ToBase64String(encrypted);
        }
    }
}
