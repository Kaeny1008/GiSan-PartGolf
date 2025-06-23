using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace T_Engine
{
    public class Cryptography
    {
        public string GetEncoding(string key, string strPasswd)
        {
            return Cryptography.EncryptData(key, strPasswd);
        }

        public string GetDecoding(string key, string strPasswd)
        {
            return Cryptography.DecryptData(key, strPasswd);
        }

        private static string EncryptData(string strKey, string strData)
        {
            ICryptoTransform transform = new DESCryptoServiceProvider
            {
                Key = Encoding.ASCII.GetBytes(strKey),
                IV = Encoding.ASCII.GetBytes(strKey)
            }.CreateEncryptor();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
            byte[] bytes = Encoding.Unicode.GetBytes(strData);
            try
            {
                cryptoStream.Write(bytes, 0, bytes.Length);
            }
            catch
            {
            }
            cryptoStream.FlushFinalBlock();
            string result;
            if (memoryStream.Length == 0L)
            {
                result = "";
            }
            else
            {
                byte[] array = memoryStream.ToArray();
                result = Convert.ToBase64String(array, 0, array.Length);
            }
            try
            {
                cryptoStream.Close();
            }
            catch
            {
            }
            return result;
        }

        private static string DecryptData(string strKey, string strData)
        {
            ICryptoTransform transform = new DESCryptoServiceProvider
            {
                Key = Encoding.ASCII.GetBytes(strKey),
                IV = Encoding.ASCII.GetBytes(strKey)
            }.CreateDecryptor();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
            char[] array = strData.ToCharArray();
            byte[] array2 = Convert.FromBase64CharArray(array, 0, array.Length);
            try
            {
                cryptoStream.Write(array2, 0, array2.Length);
            }
            catch
            {
            }
            cryptoStream.FlushFinalBlock();
            string @string = new UnicodeEncoding().GetString(memoryStream.ToArray());
            try
            {
                cryptoStream.Close();
            }
            catch
            {
            }
            return @string;
        }

        private const string CRYPTOKEY = "MIRACOMC";
    }
}
