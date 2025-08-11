namespace AssociationRegistry.Contracts;

using System.Threading;
using System.Threading.Tasks;

public interface IGrarClient
{
    Task<AddressDetailResponse> GetAddressById(string adresId, CancellationToken cancellationToken);
    Task<AdresMatchResponseCollection> GetAddressMatches(string straatnaam, string huisnummer, string busnummer, string postcode, string gemeentenaam, CancellationToken cancellationToken);
    Task<PostalInfoDetailResponse?> GetPostalInformationDetail(string postcode);
    Task<PostcodesLijstResponse> GetPostalInformationList(string offset, string limit, CancellationToken cancellationToken);
    Task<PostalNutsLauInfoResponse?> GetPostalNutsLauInformation(string postcode, CancellationToken cancellationToken);
}

public abstract class AddressDetailResponse;
public abstract class AdresMatchResponseCollection;
public abstract class PostalInfoDetailResponse;
public abstract class PostcodesLijstResponse;
public abstract class PostalNutsLauInfoResponse;
