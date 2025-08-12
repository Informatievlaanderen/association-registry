namespace AssociationRegistry.Grar;

using AssociationRegistry.Grar.Models;
using Models.PostalInfo;

public interface IGrarClient
{
    Task<AddressDetailResponse> GetAddressById(string adresId, CancellationToken cancellationToken);
    Task<AdresMatchResponseCollection> GetAddressMatches(string straatnaam, string huisnummer, string busnummer, string postcode, string gemeentenaam, CancellationToken cancellationToken);
    Task<PostalInfoDetailResponse?> GetPostalInformationDetail(string postcode);
    Task<PostcodesLijstResponse> GetPostalInformationList(string offset, string limit, CancellationToken cancellationToken);
    Task<PostalNutsLauInfoResponse?> GetPostalNutsLauInformation(string postcode, CancellationToken cancellationToken);
}
