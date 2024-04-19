namespace AssociationRegistry.Grar;

using Events;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;
using System.Net;
using Vereniging;

public class GrarClient : IGrarClient
{
    private readonly IGrarHttpClient _grarHttpClient;
    private readonly ILogger<GrarClient> _logger;

    public GrarClient(
        IGrarHttpClient grarHttpClient,
        ILogger<GrarClient> logger)
    {
        _grarHttpClient = grarHttpClient;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<AddressMatchResponse>> GetAddress(
        string straatnaam,
        string huisnummer,
        string busnummer,
        string postcode,
        string gemeentenaam)
    {
        try
        {
            var response = await _grarHttpClient.GetAddress(straatnaam, huisnummer, busnummer, postcode, gemeentenaam, CancellationToken.None);

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
                                         Adresvoorstelling: s.VolledigAdres.GeografischeNaam.Spelling,
                                         s.Straatnaam.Straatnaam.GeografischeNaam.Spelling,
                                         s.Huisnummer,
                                         s.Busnummer ,//?? "",
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
        try
        {
            var response = await _grarHttpClient.GetPostInfo(postcode, CancellationToken.None);

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
}
