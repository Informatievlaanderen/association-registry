namespace AssociationRegistry.Grar.Clients;

using Contracts;
using DecentraalBeheer.Vereniging.Adressen;
using Events;
using Exceptions;
using JasperFx.Core;
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
using Postnaam = Models.PostalInfo.Postnaam;

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
                      .Handle<AdressenRegisterReturnedTooManyRequestException>()
                      .WaitAndRetryAsync(grarClientOptions.BackoffInMs.Select(x => x.Milliseconds()));
    }

    public async Task<AddressDetailResponse> GetAddressById(string adresId, CancellationToken cancellationToken)
    {
        var result = await ExecuteGrarCall<AddressDetailOsloResponse>(
            ct => _grarHttpClient.GetAddressById(adresId, ct),
            ContextDescription.GetAddressById(adresId),
            cancellationToken);

        return new AddressDetailResponse(
            new Registratiedata.AdresId(Adresbron.AR.Code, result.Identificator.Id),
            result.AdresStatus is AdresStatus.Voorgesteld or AdresStatus.InGebruik,
            result.VolledigAdres.GeografischeNaam.Spelling,
            result.Straatnaam.Straatnaam.GeografischeNaam.Spelling,
            result.Huisnummer,
            result.Busnummer ?? string.Empty,
            result.Postinfo.ObjectId,
            result.Gemeente.Gemeentenaam.GeografischeNaam.Spelling
        );
    }

    public async Task<AdresMatchResponseCollection> GetAddressMatches(
        string straatnaam,
        string huisnummer,
        string busnummer,
        string postcode,
        string gemeentenaam,
        CancellationToken cancellationToken)
    {
        var addressMatchesResponse = await ExecuteGrarCall<AddressMatchOsloCollection>(
            ct => _grarHttpClient.GetAddressMatches(straatnaam, huisnummer, busnummer, postcode, gemeentenaam, ct),
            ContextDescription.AdresMatch(straatnaam, huisnummer, busnummer, postcode, gemeentenaam),
            cancellationToken);

        var matches = addressMatchesResponse.AdresMatches
                                            .Where(m => !string.IsNullOrEmpty(m.Identificator?.ObjectId))
                                            .Where(m => m.AdresStatus is not (AdresStatus.Gehistoreerd or AdresStatus.Afgekeurd))
                                            .Select(s => new AddressMatchResponse(
                                                        s.Score,
                                                        new Registratiedata.AdresId(Adresbron.AR.Code, s.Identificator.Id),
                                                        s.VolledigAdres.GeografischeNaam.Spelling,
                                                        s.Straatnaam.Straatnaam.GeografischeNaam.Spelling,
                                                        s.Huisnummer,
                                                        s.Busnummer ?? string.Empty,
                                                        s.Postinfo.ObjectId,
                                                        s.Gemeente.Gemeentenaam.GeografischeNaam.Spelling))
                                            .ToArray();

        return new AdresMatchResponseCollection(matches);
    }

    public async Task<PostalInfoDetailResponse?> GetPostalInformationDetail(string postcode)
    {
        var result = await ExecuteGrarCall<PostalInformationOsloResponse?>(
            ct => _grarHttpClient.GetPostInfoDetail(postcode, ct),
            ContextDescription.PostInfoDetail(postcode));

        if (result == null)
            return null;

        var gemeentenaam = result.Gemeente?.Gemeentenaam?.GeografischeNaam?.Spelling;
        var postnamen = new Postnamen(result.Postnamen.Select<Contracts.Postnaam, Postnaam>(postnaam => new Postnaam(postnaam.GeografischeNaam.Spelling)).ToList<Postnaam>());

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

    private async Task<T> ExecuteGrarCall<T>(
        Func<CancellationToken, Task<HttpResponseMessage>> httpCall,
        ContextDescription contextDescription,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await httpCall(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    ThrowMatchingException(response.StatusCode, contextDescription);
                }

                var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonConvert.DeserializeObject<T>(jsonContent)
                          ?? throw new JsonSerializationException($"Could not deserialize response to {typeof(T).Name}");;

                return result;
            });
        }
        catch (Exception ex) when (ex is JsonSerializationException or AdressenRegisterReturnedTooManyRequestException)
        {
            _logger.LogError(ex,
                             "Too many requests to {Service}. Retries exhausted for context: {Context}. Converting to non-success exception.",
                             WellKnownServices.Grar,
                             contextDescription);

            throw new AdressenregisterReturnedNonSuccessStatusCode(HttpStatusCode.InternalServerError);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex,
                             $"A timeout occurred when calling the {WellKnownServices.Grar} endpoint for {{Context}}",
                             contextDescription);

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                             $"An error occurred when calling the {WellKnownServices.Grar} endpoint for {{Context}}: {{Message}}",
                             contextDescription,
                             ex.Message);

            throw;
        }
    }

    private static (string? offset, string? limit) GetOffsetAndLimitFromUri(Uri? uri)
    {
        if (uri == null) return (null, null);

        var query = HttpUtility.ParseQueryString(uri.Query);

        return (query["offset"], query["limit"]);
    }

    private static void ThrowMatchingException(HttpStatusCode responseStatusCode, ContextDescription contextDescription)
    {
         throw responseStatusCode switch
        {
            HttpStatusCode.NotFound =>
                new AdressenregisterReturnedNotFoundStatusCode(),
            HttpStatusCode.Gone =>
                new AdressenregisterReturnedGoneStatusCode(),
            HttpStatusCode.TooManyRequests =>
                new AdressenRegisterReturnedTooManyRequestException(WellKnownServices.Grar, HttpStatusCode.TooManyRequests, contextDescription),
            _ =>
                new AdressenregisterReturnedNonSuccessStatusCode(responseStatusCode),
        };
    }
}


