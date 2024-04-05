namespace AssociationRegistry.Grar;

using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;

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

    public async Task<IReadOnlyCollection<AddressMatchResponse>> GetAddress(string straatnaam, string huisnummer, string busnummer, string postcode, string gemeentenaam)
    {
        using var client = GetHttpClient();

        try
        {
            var response = await client.GetAddress(straatnaam, huisnummer, busnummer, postcode, gemeentenaam, CancellationToken.None);

            var result = JsonConvert.DeserializeObject<AddressMatchOsloCollection>(await response.Content.ReadAsStringAsync());

            var matches = result.AdresMatches.Select(s => new AddressMatchResponse(
                                                         Score: s.Score,
                                                         AdresId: s.Identificator.ObjectId,
                                                         AdresStatus: s.AdresStatus,
                                                         s.Straatnaam.Straatnaam.GeografischeNaam.Spelling,
                                                         s.Huisnummer,
                                                         s.Busnummer,
                                                         s.Postinfo.ObjectId,
                                                         s.Gemeente.Gemeentenaam.GeografischeNaam.Spelling
                                                     )).ToArray();

            return matches;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, message: "{Message}", ex.Message);

            throw new Exception(message: "A timeout occurred when calling the Magda services", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, message: "An error occurred when calling the Magda services: {Message}", ex.Message);

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
