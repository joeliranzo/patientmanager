namespace PatientManagement.Infrastructure.Security;

public interface IEncryptionService
{
    byte[] EncryptToBytes(string plainText);
    string DecryptToString(byte[] cipherData);
}