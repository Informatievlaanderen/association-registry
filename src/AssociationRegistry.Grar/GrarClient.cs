namespace AssociationRegistry.Grar;

using Configuration;
using Microsoft.Extensions.Logging;

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

    public async Task GetAddress(string gemeentenaam, string straatnaam, string huisNummer)
    {
        using var client = GetHttpClient();

        try
        {
            await client.GetAddress(gemeentenaam, straatnaam, huisNummer, CancellationToken.None);
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
