namespace AssociationRegistry.Grar.Clients;

using AssociationRegistry.Grar.Models.PostalInfo;
using Models;

public interface IGrarClient
{
    Task<AddressDetailResponse> GetAddressById(string adresId, CancellationToken cancellationToken);
    Task<AdresMatchResponseCollection> GetAddressMatches(string straatnaam, string huisnummer, string busnummer, string postcode, string gemeentenaam, CancellationToken cancellationToken);
    Task<PostalInformationResponse> GetPostalInformation(string postcode);
}
