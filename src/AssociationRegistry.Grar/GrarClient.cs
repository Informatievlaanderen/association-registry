namespace AssociationRegistry.Grar;

using Configuration;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text.Json;

public class GrarClient : IGrarClient
{
    private readonly GrarOptionsSection _grarOptions;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly ILogger<GrarClient> _logger;

    public GrarClient(
        GrarOptionsSection grarOptions,
        ILogger<GrarClient> logger)
    {
        _grarOptions = grarOptions;
        _jsonSerializerOptions = new JsonSerializerOptions();
        _logger = logger;
    }

    public async Task GetAddress(string gemeentenaam, string straatnaam, string huisNummer)
    {
        using var client = GetHttpClient();

        try
        {
            var response =  await client.GetAddress(gemeentenaam, straatnaam, huisNummer, CancellationToken.None);

           // var result = await response.Content.ReadFromJsonAsync<AddressMatchOsloCollection>(_jsonSerializerOptions);
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
