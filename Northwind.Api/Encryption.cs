using System.Security.Cryptography;
using System.Text;

namespace Northwind.Api
{
    public static class EncryptionUtility
    {
        public static string EncryptString(string plainText, string password)
        {
            byte[] salt = new byte[16];
            RandomNumberGenerator.Fill(salt);

            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] key = deriveBytes.GetBytes(32);

            byte[] nonce = new byte[12];
            RandomNumberGenerator.Fill(nonce);

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = new byte[plainBytes.Length];
            byte[] tag = new byte[16];

            using (var aesGcm = new AesGcm(key))
            {
                aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);
            }

            byte[] result = new byte[salt.Length + nonce.Length + tag.Length + cipherBytes.Length];
            Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
            Buffer.BlockCopy(nonce, 0, result, salt.Length, nonce.Length);
            Buffer.BlockCopy(tag, 0, result, salt.Length + nonce.Length, tag.Length);
            Buffer.BlockCopy(cipherBytes, 0, result, salt.Length + nonce.Length + tag.Length, cipherBytes.Length);

            return Convert.ToBase64String(result);
        }

        public static string DecryptString(string cipherText, string password)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            byte[] salt = new byte[16];
            Buffer.BlockCopy(fullCipher, 0, salt, 0, salt.Length);

            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] key = deriveBytes.GetBytes(32);

            byte[] nonce = new byte[12];
            Buffer.BlockCopy(fullCipher, salt.Length, nonce, 0, nonce.Length);

            var tag = new byte[16];
            Buffer.BlockCopy(fullCipher, salt.Length + nonce.Length, tag, 0, tag.Length);

            int cipherLength = fullCipher.Length - salt.Length - nonce.Length - tag.Length;
            byte[] cipherBytes = new byte[cipherLength];
            Buffer.BlockCopy(fullCipher, salt.Length + nonce.Length + tag.Length, cipherBytes, 0, cipherLength);

            byte[] plainBytes = new byte[cipherLength];

            using (var aesGcm = new AesGcm(key))
            {
                aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);
            }

            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
