using System;
using System.Security.Cryptography;
using System.Text;

namespace GisanParkGolf.Security
{
    /// <summary>
    /// PBKDF2 (HMACSHA256) 기반 비밀번호 해시 유틸리티.
    /// 저장 포맷: PBKDF2${iterations}${saltBase64}${hashBase64}
    /// </summary>
    public class PasswordHasher
    {
        private const int SaltSize = 16; // bytes
        private const int KeySize = 32; // bytes (256 bit)
        private const int DefaultIterations = 100_000; // 권장 최소값: 100k (환경에 따라 조정)

        public string Hash(string password, int iterations = DefaultIterations)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            var salt = new byte[SaltSize];
            RandomNumberGenerator.Fill(salt);

            using var derive = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var key = derive.GetBytes(KeySize);

            return $"PBKDF2${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
        }

        public bool Verify(string password, string storedHash)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrEmpty(storedHash)) return false;

            // storedHash expected format: PBKDF2${iterations}${saltBase64}${hashBase64}
            var parts = storedHash.Split('$');
            if (parts.Length != 4) return false;
            if (!parts[0].Equals("PBKDF2", StringComparison.Ordinal)) return false;

            if (!int.TryParse(parts[1], out var iterations)) return false;
            var salt = Convert.FromBase64String(parts[2]);
            var key = Convert.FromBase64String(parts[3]);

            using var derive = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var keyToCheck = derive.GetBytes(key.Length);

            // 안전한 타이밍 일관 비교
            return CryptographicOperations.FixedTimeEquals(keyToCheck, key);
        }

        public bool IsHashFormat(string storedHash)
        {
            return !string.IsNullOrEmpty(storedHash) && storedHash.StartsWith("PBKDF2$", StringComparison.Ordinal);
        }
    }
}