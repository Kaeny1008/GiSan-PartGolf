using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace T_Engine
{
    public class PasswordPolicy
    {
        public static string HashedEmptyPassword
        {
            get
            {
                string result;
                if ((result = PasswordPolicy._hashedEmptyPassword) == null)
                {
                    result = (PasswordPolicy._hashedEmptyPassword = new PasswordPolicy().HashPassword(""));
                }
                return result;
            }
        }

        public string HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            if (!PasswordPolicy.IsPasswordHashed)
            {
                return password;
            }
            return this.HashPasswordForth(password);
        }

        public string HashPasswordForth(string password)
        {
            byte[] bytes = this._utf8Encoding.GetBytes(password + "t-solution");
            return Convert.ToBase64String(this._sha256.ComputeHash(bytes));
        }

        public static bool IsPasswordHashedDBVer(double dbVersion, int nPackageType)
        {
            try
            {
                if (nPackageType == 1)
                {
                    return Convert.ToDouble(PasswordPolicy.MinDBVersionForHashedPasswordT1.ToString()) <= dbVersion;
                }
                if (nPackageType == 2 || nPackageType == 3)
                {
                    return Convert.ToDouble(PasswordPolicy.MinDBVersionForHashedPasswordT2.ToString()) <= dbVersion;
                }
            }
            catch
            {
            }
            return false;
        }

        public static PasswordStrength CheckPasswordStrength(string password)
        {
            if (password.Length < 8)
            {
                return PasswordStrength.VeryWeak;
            }
            int num = 0;
            if (password.Length >= 14)
            {
                num++;
            }
            if (Regex.IsMatch(password, "[a-z]+"))
            {
                num++;
            }
            if (Regex.IsMatch(password, "[A-Z]+"))
            {
                num++;
            }
            if (Regex.IsMatch(password, "\\d+"))
            {
                num++;
            }
            if (Regex.IsMatch(password, "[^a-zA-Z\\d]+"))
            {
                num++;
            }
            if (num == 0)
            {
                return PasswordStrength.VeryWeak;
            }
            if (num == 1)
            {
                return PasswordStrength.Weak;
            }
            if (num == 2)
            {
                return PasswordStrength.Medium;
            }
            if (num == 3)
            {
                return PasswordStrength.Strong;
            }
            return PasswordStrength.VeryStrong;
        }

        public static bool IsPasswordChangeRecommended(string password)
        {
            switch (PasswordPolicy.CheckPasswordStrength(password))
            {
                case PasswordStrength.VeryWeak:
                case PasswordStrength.Weak:
                case PasswordStrength.Medium:
                    return true;
                default:
                    return false;
            }
        }

        private static string _hashedEmptyPassword;

        public static bool IsPasswordHashed = false;

        private UTF8Encoding _utf8Encoding = new UTF8Encoding();

        private SHA256 _sha256 = new SHA256Managed();

        private static readonly Version MinDBVersionForHashedPasswordT1 = new Version("1.48");

        public static readonly Version MinDBVersionForHashedPasswordT2 = new Version("2.71");
    }

    public enum PasswordStrength
    {
        VeryWeak,
        Weak,
        Medium,
        Strong,
        VeryStrong
    }
}
