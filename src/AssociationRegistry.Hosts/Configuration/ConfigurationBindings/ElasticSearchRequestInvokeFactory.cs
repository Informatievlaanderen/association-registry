namespace AssociationRegistry.Hosts.Configuration.ConfigurationBindings;

using System.Net;
using Elastic.Transport;

public static class ElasticSearchRequestInvokerFactory
{
    /// <summary>
    /// De AWS ALB voor Elasticsearch sluit verbindingen na 60 s idle.
    /// De .NET-default voor PooledConnectionIdleTimeout is ook 60 s, wat een race veroorzaakt:
    /// de client hergebruikt een verbinding die de ALB net gesloten heeft ("The response ended prematurely").
    /// Hou PooledConnectionIdleTimeoutInSeconds daarom ruim onder de idle timeout van de ALB.
    /// </summary>
    public static HttpRequestInvoker Create(ElasticSearchOptionsSection options)
    {
        var idleTimeout = TimeSpan.FromSeconds(options.PooledConnectionIdleTimeoutInSeconds);
        var lifetime = TimeSpan.FromSeconds(options.PooledConnectionLifetimeInSeconds);

        return new HttpRequestInvoker(
            (defaultHandler, _) =>
            {
                defaultHandler.Dispose();

                return new SocketsHttpHandler
                {
                    PooledConnectionIdleTimeout = idleTimeout,
                    PooledConnectionLifetime = lifetime,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    SslOptions = { RemoteCertificateValidationCallback = (_, _, _, _) => true },
                };
            }
        );
    }
}
