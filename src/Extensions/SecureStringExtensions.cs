// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace DiabloSpeech.Extensions
{
    public static class SecureStringExtensions
    {
        public static string ToUnsecureString(this SecureString secure)
        {
            if (secure == null)
                throw new ArgumentNullException(nameof(secure));
            IntPtr unmanaged = IntPtr.Zero;
            try
            {
                unmanaged = Marshal.SecureStringToGlobalAllocUnicode(secure);
                return Marshal.PtrToStringUni(unmanaged);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanaged);
            }
        }
    }
}
