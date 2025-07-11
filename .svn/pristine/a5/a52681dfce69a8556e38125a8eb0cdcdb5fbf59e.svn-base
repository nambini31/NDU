using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.ServiceEncryptor
{
    public class Encryption
    {

        private static string key = "1234567890123456"; // 16 octets (128 bits)
        private static string iv = "1234567890123456";  // 16 octets (128 bits)

        // Méthode pour crypter le texte
        public static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    // Convertir en Base64 et supprimer les "==" de fin
                    return Convert.ToBase64String(msEncrypt.ToArray()).TrimEnd('=');
                }
            }
        }

        // Méthode pour décrypter le texte
        public static string Decrypt(string cipherText)
        {
            // Ajouter les "==" à la fin du texte crypté pour le décodage Base64
            int padding = cipherText.Length % 4;
            if (padding > 0)
            {
                cipherText = cipherText.PadRight(cipherText.Length + (4 - padding), '=');
            }

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }

}
