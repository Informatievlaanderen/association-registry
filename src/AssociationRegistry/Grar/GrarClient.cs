namespace AssociationRegistry.Grar;

using Events;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;
using System.Net;
using Vereniging;

public class GrarClient : IGrarClient
{
    private readonly GrarOptionsSection _grarOptions;
    private readonly ILogger<GrarClient> _logger;

    public GrarClient(
        GrarOptionsSection grarOptions,
        ILogger<GrarClient> logger)
    {
        _grarOptions = grarOptions;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<AddressMatchResponse>> GetAddress(
        string straatnaam,
        string huisnummer,
        string busnummer,
        string postcode,
        string gemeentenaam)
    {
        using var client = GetHttpClient();

        try
        {
            var response = await client.GetAddress(straatnaam, huisnummer, busnummer, postcode, gemeentenaam, CancellationToken.None);

            return response.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<AddressMatchOsloCollection>(await response.Content.ReadAsStringAsync())
                             .AdresMatches
                             .Where(w => !string.IsNullOrEmpty(w.Identificator.ObjectId))
                             .Where(w => w.AdresStatus != AdresStatus.Gehistoreerd)
                             .Select(s => new AddressMatchResponse(
                                         Score: s.Score,
                                         AdresId: new Registratiedata.AdresId(
                                             Adresbron.AR.Code,
                                             s.Identificator.Id
                                         ),
                                         AdresStatus: s.AdresStatus,
                                         s.Straatnaam.Straatnaam.GeografischeNaam.Spelling,
                                         s.Huisnummer,
                                         s.Busnummer,
                                         s.Postinfo.ObjectId,
                                         s.Gemeente.Gemeentenaam.GeografischeNaam.Spelling
                                     )).ToArray()
                : Array.Empty<AddressMatchResponse>();
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, message: "{Message}", ex.Message);

            throw new Exception(message: "A timeout occurred when calling the address match endpoint", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, message: "An error occurred when calling the address match endpoint: {Message}", ex.Message);

            throw new Exception(ex.Message, ex);
        }
    }

    public async Task<PostalInformationResponse?> GetPostalInformation(string postcode)
    {
        using var client = GetHttpClient();

        try
        {
            var response = await client.GetPostInfo(postcode, CancellationToken.None);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<PostalInformationOsloResponse>(jsonContent);

                    var postalInformationResponse = new PostalInformationResponse(postcode,
                                                                                  result.Gemeente.Gemeentenaam.GeografischeNaam.Spelling,
                                                                                  result.Postnamen.Select(s => s.GeografischeNaam.Spelling)
                                                                                        .ToArray());

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

    private GrarHttpClient GetHttpClient()
    {
        var client = new GrarHttpClient(new HttpClient()
        {
            BaseAddress = new Uri(_grarOptions.BaseUrl)
        });

        return client;
    }
}
