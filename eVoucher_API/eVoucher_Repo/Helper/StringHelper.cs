﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace eVoucher_Repo.Helper
{
    public static class StringHelper
    {
        public static string GenerateHash(string plainText)
        {
            byte[] salt = new byte[128 / 8];

            if (String.IsNullOrEmpty(plainText))
            {
                return String.Empty;
            }
            return Base64Encode(plainText);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
