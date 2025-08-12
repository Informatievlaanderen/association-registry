namespace AssociationRegistry.Integrations.Grar.Clients;

using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

public class GrarHttpClientHandler : HttpClientHandler
{
    public GrarHttpClientHandler(X509Certificate? clientCertificate)
    {
        ClientCertificateOptions = ClientCertificateOption.Manual;
        SslProtocols = SslProtocols.Tls12;

        if (clientCertificate is not null)
            ClientCertificates.Add(clientCertificate);
    }
}
