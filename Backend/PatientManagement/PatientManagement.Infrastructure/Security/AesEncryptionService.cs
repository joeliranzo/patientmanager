using System.Security.Cryptography;
using System.Text;

namespace PatientManagement.Infrastructure.Security;

public class AesEncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    public AesEncryptionService(string key)
    {
        _key = Encoding.UTF8.GetBytes(key);
        if (_key.Length != 16 && _key.Length != 24 && _key.Length != 32)
        {
            throw new ArgumentException("Invalid key size for AES. Must be 128, 192, or 256 bits.");
        }
    }

    public byte[] EncryptToBytes(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return Array.Empty<byte>();

        using var aesAlg = Aes.Create();
        aesAlg.Key = _key;
        aesAlg.GenerateIV();
        var iv = aesAlg.IV;

        using var msEncrypt = new MemoryStream();
        msEncrypt.Write(iv, 0, iv.Length);

        using (var csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        return msEncrypt.ToArray();
    }

    public string DecryptToString(byte[] cipherData)
    {
        if (cipherData == null || cipherData.Length == 0)
            return string.Empty;

        using var aesAlg = Aes.Create();
        aesAlg.Key = _key;

        var iv = new byte[aesAlg.BlockSize / 8];
        Array.Copy(cipherData, 0, iv, 0, iv.Length);
        aesAlg.IV = iv;

        using var msDecrypt = new MemoryStream(cipherData, iv.Length, cipherData.Length - iv.Length);
        using var csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        return srDecrypt.ReadToEnd();
    }
}