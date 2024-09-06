using System;
using System.Security.Cryptography;

namespace DCI.Core.Helpers
{
    public static class TokenGeneratorHelper
    {
        public static string ResetPasswordToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var data = new byte[32];
                rng.GetBytes(data);
                return Convert.ToBase64String(data);
            }
        }
    }
}
