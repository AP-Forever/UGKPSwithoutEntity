using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace UGKPSwithoutEntity.Useful_Classes
{
    public static class EncryptPW
    {
        public static string Hash(string value)
        {
            byte[] hash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(value));
            string hashedPW = Convert.ToBase64String(hash);
            return hashedPW;
        }
    }
}