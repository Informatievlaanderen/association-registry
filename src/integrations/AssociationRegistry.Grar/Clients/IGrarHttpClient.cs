namespace AssociationRegistry.Grar.Clients;

public interface IGrarHttpClient : IDisposable
{
    Task<HttpResponseMessage> GetAddressById(string adresId, CancellationToken cancellationToken);

    Task<HttpResponseMessage> GetAddressMatches(
        string straatnaam,
        string huisnummer,
        string busnummer,
        string postcode,
        string gemeentenaam,
        CancellationToken cancellationToken);

    Task<HttpResponseMessage> GetPostInfoDetail(string postcode, CancellationToken cancellationToken);
    Task<HttpResponseMessage> GetPostInfoList(string offset, string limit, CancellationToken cancellationToken);
}
