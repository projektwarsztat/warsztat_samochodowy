using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PasswordCryptography
{
    /// <summary>
    /// Klasa zajmująca się obsługą szyfrowania i deszyfrowania hasła z wykorzystaniem algorytmu symetrycznego TripleDES
    /// </summary>
    public class passwordCryptography
    {
        private static string key = "mnyw-5uec-d3r2zk"; //Klucz używany w kodowaniu

        /// <summary>
        /// Szyfruje hasło za pomocą symetrycznego algorytmu CBC
        /// </summary>
        /// <returns>Zakodowane hasło</returns>
        public static string Encrypt(string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data); //Zakodowanie czystego hasła w ciąg znaków w formacie UTF-8
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key); //Ustawienie klucza 
            tripleDES.Mode = CipherMode.ECB; //Ustawienie metody szyfrującej
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cryptoTransform = tripleDES.CreateEncryptor();
            byte[] resultBytes = cryptoTransform.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultBytes, 0, resultBytes.Length);
        }

        /// <summary>
        /// Deszyfruje hasło za pomocą symetrycznego algorytmu CBC
        /// </summary>
        /// <returns>Rozkodowane hasło</returns>
        public static string Decrypt(string data)
        {
            byte[] dataBytes = Convert.FromBase64String(data);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cryptoTransform = tripleDES.CreateDecryptor();
            byte[] resultBytes = cryptoTransform.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultBytes);
        }
    }
}
