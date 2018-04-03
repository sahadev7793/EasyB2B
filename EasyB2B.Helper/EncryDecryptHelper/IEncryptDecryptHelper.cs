using System;
using System.Collections.Generic;
using System.Text;

namespace EasyB2B.Helper.EncryDecryHelper
{
    public interface IEncryptDecryptHelper
    {
        string Encrypt<T>(T obj);
        string Encrypt(string plaintext);
        bool TryDecrypt<T>(string encryptedText, out T obj);
        bool TryDecrypt(string encryptedText, out string decryptedText);
    }
}
