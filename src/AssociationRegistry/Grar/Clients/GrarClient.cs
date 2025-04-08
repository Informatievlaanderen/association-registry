namespace AssociationRegistry.Grar.Clients;

using AssociationRegistry.Grar.Exceptions;
using AssociationRegistry.Grar.Models.PostalInfo;
using AssociationRegistry.Resources;
using AssociationRegistry.Vereniging;
using Contracts;
using Events;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;
using System.Net;
using System.Web;
using Weasel.Postgresql.Tables.Partitioning;

public class GrarClient : IGrarClient
{
    private readonly IGrarHttpClient _grarHttpClient;
    private readonly ILogger<GrarClient> _logger;
    public const string BadRequestSuccessStatusCodeMessage = "Foutieve request.";
    public const string OtherNonSuccessStatusCodeMessage = "Adressenregister niet bereikbaar.";

    public GrarClient(
        IGrarHttpClient grarHttpClient,
        ILogger<GrarClient> logger)
    {
        _grarHttpClient = grarHttpClient;
        _logger = logger;
    }

    public async Task<AddressDetailResponse> GetAddressById(string adresId, CancellationToken cancellationToken)
    {
        try
        {
            var response =
                await _grarHttpClient.GetAddressById(adresId, CancellationToken.None);

            if (!response.IsSuccessStatusCode)
            {
                throw response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => new AdressenregisterReturnedClientErrorStatusCode(
                        response.StatusCode, ExceptionMessages.AdresKonNietGevalideerdWordenBijAdressenregister),
                    HttpStatusCode.NotFound => new AdressenregisterReturnedClientErrorStatusCode(
                        response.StatusCode, ExceptionMessages.AdresKonNietGevalideerdWordenBijAdressenregister),
                    HttpStatusCode.Gone => new AdressenregisterReturnedGoneStatusCode(),
                    _ => new AdressenregisterReturnedNonSuccessStatusCode(response.StatusCode)
                };
            }

            var addressDetailOsloResponse =
                JsonConvert.DeserializeObject<AddressDetailOsloResponse>(await response.Content.ReadAsStringAsync(cancellationToken));

            return new AddressDetailResponse(new Registratiedata.AdresId(
                                                 Adresbron.AR.Code,
                                                 addressDetailOsloResponse.Identificator.Id
                                             ),
                                             addressDetailOsloResponse.AdresStatus is AdresStatus.Voorgesteld or AdresStatus.InGebruik,
                                             addressDetailOsloResponse.VolledigAdres.GeografischeNaam.Spelling,
                                             addressDetailOsloResponse.Straatnaam.Straatnaam.GeografischeNaam.Spelling,
                                             addressDetailOsloResponse.Huisnummer,
                                             addressDetailOsloResponse.Busnummer ?? string.Empty,
                                             addressDetailOsloResponse.Postinfo.ObjectId,
                                             addressDetailOsloResponse.Gemeente.Gemeentenaam.GeografischeNaam.Spelling
            );
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, message: "{Message}", ex.Message);

            throw new Exception(message: "A timeout occurred when calling the address match endpoint", ex);
        }
        catch (AdressenregisterReturnedGoneStatusCode ex)
        {
            _logger.LogError(ex, message: "A gone status code occurred when calling the address match endpoint: {Message}",
                             ex.Message);

            throw;
        }
        catch (AdressenregisterReturnedNonSuccessStatusCode ex)
        {
            _logger.LogError(ex, message: "An non-success status code occurred when calling the address match endpoint: {Message}",
                             ex.Message);

            throw;
        }
        catch (AdressenregisterReturnedClientErrorStatusCode ex)
        {
            _logger.LogError(ex, message: "A client error status code occurred when calling the address match endpoint: {Message}",
                             ex.Message);

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, message: "An error occurred when calling the address match endpoint: {Message}", ex.Message);

            throw new Exception(ex.Message, ex);
        }
    }

    public async Task<AdresMatchResponseCollection> GetAddressMatches(
        string straatnaam,
        string huisnummer,
        string busnummer,
        string postcode,
        string gemeentenaam,
        CancellationToken cancellationToken)
    {
        try
        {
            var response =
                await _grarHttpClient.GetAddressMatches(straatnaam, huisnummer, busnummer, postcode, gemeentenaam, cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new AdressenregisterReturnedNonSuccessStatusCode(response.StatusCode);

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            var addressMatchOsloCollection = JsonConvert.DeserializeObject<AddressMatchOsloCollection>(content)
                                                        .AdresMatches
                                                        .Where(w => !string.IsNullOrEmpty(w.Identificator?.ObjectId))
                                                        .Where(w => w.AdresStatus != AdresStatus.Gehistoreerd &&
                                                                    w.AdresStatus != AdresStatus.Afgekeurd)
                                                        .ToList();

            if (addressMatchOsloCollection.Count == 0)
                return new AdresMatchResponseCollection([]);

            return new AdresMatchResponseCollection(addressMatchOsloCollection.Select(s => new AddressMatchResponse(
                                                                                          Score: s.Score,
                                                                                          AdresId: new Registratiedata.AdresId(
                                                                                              Adresbron.AR.Code,
                                                                                              s.Identificator.Id
                                                                                          ),
                                                                                          Adresvoorstelling: s.VolledigAdres
                                                                                             .GeografischeNaam.Spelling,
                                                                                          s.Straatnaam.Straatnaam.GeografischeNaam.Spelling,
                                                                                          s.Huisnummer,
                                                                                          s.Busnummer ?? string.Empty,
                                                                                          s.Postinfo.ObjectId,
                                                                                          s.Gemeente.Gemeentenaam.GeografischeNaam.Spelling
                                                                                      )).ToArray());
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, message: "{Message}", ex.Message);

            throw new Exception(message: "A timeout occurred when calling the address match endpoint", ex);
        }
        catch (AdressenregisterReturnedNonSuccessStatusCode ex)
        {
            _logger.LogError(ex, message: "An non-success status code occurred when calling the address match endpoint: {Message}",
                             ex.Message);

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, message: "An error occurred when calling the address match endpoint: {Message}", ex.Message);

            throw new Exception(ex.Message, ex);
        }
    }

    public async Task<PostalInfoDetailResponse?> GetPostalInformationDetail(string postcode)
    {
        try
        {
            var response = await _grarHttpClient.GetPostInfoDetail(postcode, CancellationToken.None);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<PostalInformationOsloResponse>(jsonContent);

                    var gemeentenaam = result.Gemeente?.Gemeentenaam?.GeografischeNaam?.Spelling;
                    var postnamen = Postnamen.FromPostalInfo(result.Postnamen);

                    var postalInformationResponse = new PostalInfoDetailResponse(postcode,
                                                                                 gemeentenaam ?? postnamen[0],
                                                                                 postnamen);

                    return postalInformationResponse;
                }

                case HttpStatusCode.NotFound:
                default:
                    return null;
            }
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, message: "{Message}", ex.Message);

            throw new Exception(message: "A timeout occurred when calling the postal information endpoint", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, message: "An error occurred when calling the postal information endpoint: {Message}", ex.Message);

            throw new Exception(ex.Message, ex);
        }
    }

    public async Task<PostalNutsLauInfoResponse?> GetPostalNutsLauInformation(string postcode, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _grarHttpClient.GetPostInfoDetail(postcode, cancellationToken);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                {
                    var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    var result = JsonConvert.DeserializeObject<PostalInformationOsloResponse>(jsonContent);

                    var gemeentenaam = result.Gemeente?.Gemeentenaam?.GeografischeNaam?.Spelling;
                    var nuts = result.Nuts3Code;
                    var lau = result.Gemeente?.ObjectId;

                    if (gemeentenaam == null || nuts == null || lau is null)
                    {
                        return null;
                    }

                    var postalInformationResponse = new PostalNutsLauInfoResponse(postcode, gemeentenaam, nuts, lau);

                    return postalInformationResponse;
                }

                case HttpStatusCode.NotFound:
                default:
                    return null;
            }
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, message: "{Message}", ex.Message);

            throw new Exception(message: "A timeout occurred when calling the postal information endpoint", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, message: "An error occurred when calling the postal information endpoint: {Message}", ex.Message);

            throw new Exception(ex.Message, ex);
        }
    }

    public async Task<PostcodesLijstResponse?> GetPostalInformationList(string offset, string limit, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _grarHttpClient.GetPostInfoList(offset, limit, cancellationToken);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<PostalInformationListOsloResponse>(jsonContent);

                    if (result is null)
                        return null;

                    var (nextOffset, nextLimit) = GetOffsetAndLimitFromUri(result.Volgende);

                    return new PostcodesLijstResponse()
                    {
                        Postcodes = result.PostInfoObjecten.Select(x => x.Identificator.ObjectId).ToArray(),
                        VolgendeOffset = nextOffset,
                        VolgendeLimit = nextLimit,
                    };
                }

                case HttpStatusCode.NotFound:
                default:
                    return null;
            }
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, message: "{Message}", ex.Message);

            throw new Exception(message: "A timeout occurred when calling the postal information endpoint", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, message: "An error occurred when calling the postal information endpoint: {Message}", ex.Message);

            throw new Exception(ex.Message, ex);
        }
    }

    private static (string? offset, string? limit) GetOffsetAndLimitFromUri(Uri? uri)
    {
        if (uri == null) return (null, null);

        var query = HttpUtility.ParseQueryString(uri.Query);

        return (query["offset"], query["limit"]);
    }
}
