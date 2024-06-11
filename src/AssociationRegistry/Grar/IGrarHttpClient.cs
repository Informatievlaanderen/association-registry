namespace AssociationRegistry.Grar;

public interface IGrarHttpClient : IDisposable
{
    Task<HttpResponseMessage> GetAddress(string adresId, CancellationToken cancellationToken);

    Task<HttpResponseMessage> GetAddress(
        string straatnaam,
        string huisnummer,
        string busnummer,
        string postcode,
        string gemeentenaam,
        CancellationToken cancellationToken);

    Task<HttpResponseMessage> GetPostInfo(string postcode, CancellationToken cancellationToken);
}
