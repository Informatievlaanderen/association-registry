namespace AssociationRegistry.Integrations.Magda;

using System.Security.Cryptography.X509Certificates;

public class MagdaClientCertificate : X509Certificate2
{

    private MagdaClientCertificate(string base64Cert, string password) :
        base(
            Convert.FromBase64String(base64Cert),
            password,
            GetKeyStorageFlags())
    {
    }

    private static X509KeyStorageFlags GetKeyStorageFlags()
    {
        if(OperatingSystem.IsWindows())
            return X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.PersistKeySet;

        return X509KeyStorageFlags.MachineKeySet |
               X509KeyStorageFlags.PersistKeySet |
               X509KeyStorageFlags.Exportable;
    }

    public static MagdaClientCertificate Create(string base64Cert, string password)
        => new(base64Cert, password);
}
