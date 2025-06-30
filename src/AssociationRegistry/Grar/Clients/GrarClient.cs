namespace AssociationRegistry.Grar.Clients;

using Contracts;
using Events;
using Exceptions;
using Humanizer;
using Microsoft.Extensions.Logging;
using Models;
using Models.PostalInfo;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Resources;
using System.Net;
using System.Web;
using Vereniging;

public class GrarClient : IGrarClient
{
    private readonly IGrarHttpClient _grarHttpClient;
    private readonly ILogger<GrarClient> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    public const string BadRequestSuccessStatusCodeMessage = "Foutieve request.";
    public const string OtherNonSuccessStatusCodeMessage = "Adressenregister niet bereikbaar.";

    public GrarClient(
        IGrarHttpClient grarHttpClient,
        GrarOptions.GrarClientOptions grarClientOptions,
        ILogger<GrarClient> logger)
    {
        _grarHttpClient = grarHttpClient;
        _logger = logger;

        _retryPolicy = Policy
                      .Handle<TooManyRequestException>()
                      .WaitAndRetryAsync(grarClientOptions.BackoffInMs.Select(x => x.Milliseconds()));
    }

    public async Task<AddressDetailResponse> GetAddressById(string adresId, CancellationToken cancellationToken)
    {
        try
        {
            var response =
                await _grarHttpClient.GetAddressById(adresId, CancellationToken.None);

            if (!response.IsSuccessStatusCode)
            {
                ThrowMatchingException(response.StatusCode);
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

    private static void ThrowMatchingException(HttpStatusCode responseStatusCode)
    {
        throw responseStatusCode switch
        {
            HttpStatusCode.BadRequest =>
                new AdressenregisterReturnedClientErrorStatusCode(
                    responseStatusCode, ExceptionMessages.AdresKonNietGevalideerdWordenBijAdressenregister),
            HttpStatusCode.NotFound =>
                new AdressenregisterReturnedClientErrorStatusCode(
                    responseStatusCode, ExceptionMessages.AdresKonNietGevalideerdWordenBijAdressenregister),
            HttpStatusCode.Gone =>
                new AdressenregisterReturnedGoneStatusCode(),
            _ =>
                new AdressenregisterReturnedNonSuccessStatusCode(responseStatusCode)
        };
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
        var result = await ExecuteGrarCall<PostalInformationOsloResponse?>(
            ct => _grarHttpClient.GetPostInfoDetail(postcode, ct),
            ContextDescription.PostInfoDetail(postcode));

        if (result == null)
            return null;

        var gemeentenaam = result.Gemeente?.Gemeentenaam?.GeografischeNaam?.Spelling;
        var postnamen = Postnamen.FromPostalInfo(result.Postnamen);

        return new PostalInfoDetailResponse(postcode, gemeentenaam ?? postnamen[0], postnamen);
    }

    public async Task<PostalNutsLauInfoResponse?> GetPostalNutsLauInformation(string postcode, CancellationToken cancellationToken)
    {
        var result = await ExecuteGrarCall<PostalInformationOsloResponse>(
            ct => _grarHttpClient.GetPostInfoDetail(postcode, ct),
            ContextDescription.PostInfoDetail(postcode),
            cancellationToken);

        var gemeentenaam = result.Gemeente?.Gemeentenaam?.GeografischeNaam?.Spelling;
        var nuts = result.Nuts3Code;
        var lau = result.Gemeente?.ObjectId;

        if (gemeentenaam == null || nuts == null || lau is null)
        {
            _logger.LogInformation("grar gemeentenaam or nuts/lau is null for postcode: {Postcode}", postcode);

            return null;
        }

        return new PostalNutsLauInfoResponse(postcode, gemeentenaam, nuts, lau);
    }

    public async Task<PostcodesLijstResponse> GetPostalInformationList(string offset, string limit, CancellationToken cancellationToken)
    {
        var result = await ExecuteGrarCall<PostalInformationListOsloResponse>(
            ct => _grarHttpClient.GetPostInfoList(offset, limit, ct),
            ContextDescription.PostInfoLijst(offset, limit),
            cancellationToken);

        var (nextOffset, nextLimit) = GetOffsetAndLimitFromUri(result.Volgende);

        return new PostcodesLijstResponse
        {
            Postcodes = result.PostInfoObjecten.Select(x => x.Identificator.ObjectId).ToArray(),
            VolgendeOffset = nextOffset,
            VolgendeLimit = nextLimit,
        };
    }

    private async Task<T?> ExecuteGrarCall<T>(
        Func<CancellationToken, Task<HttpResponseMessage>> httpCall,
        ContextDescription contextDescription,
        CancellationToken cancellationToken = default)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                var response = await httpCall(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.TooManyRequests)
                        throw new TooManyRequestException(WellKnownServices.Grar, response.StatusCode, contextDescription);

                    throw new NonSuccesfulStatusCodeException(WellKnownServices.Grar, response.StatusCode, contextDescription);
                }

                var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonConvert.DeserializeObject<T>(jsonContent);

                return result;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, $"A timeout occurred when calling the {WellKnownServices.Grar} endpoint for {{Context}}",
                                 contextDescription);

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred when calling the {WellKnownServices.Grar} endpoint for {{Context}}: {{Message}}",
                                 contextDescription,
                                 ex.Message);

                throw;
            }
        });
    }

    private static (string? offset, string? limit) GetOffsetAndLimitFromUri(Uri? uri)
    {
        if (uri == null) return (null, null);

        var query = HttpUtility.ParseQueryString(uri.Query);

        return (query["offset"], query["limit"]);
    }
}

public class TooManyRequestException(string service, HttpStatusCode statusCode, ContextDescription contextDescription)
    : Exception(FormattedExceptionMessages.ServiceReturnedNonSuccesfulStatusCode(service, statusCode, contextDescription));

public class NonSuccesfulStatusCodeException(string service, HttpStatusCode statusCode, ContextDescription contextDescription)
    : Exception(FormattedExceptionMessages.ServiceReturnedNonSuccesfulStatusCode(service, statusCode, contextDescription));
