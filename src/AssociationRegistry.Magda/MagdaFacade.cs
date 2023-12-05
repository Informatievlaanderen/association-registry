namespace AssociationRegistry.Magda;

using Configuration;
using Framework;
using Microsoft.Extensions.Logging;
using Models;
using Models.GeefOnderneming;
using Models.GeefOndernemingVKBO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;

public class MagdaFacade : IMagdaFacade
{
    private readonly MagdaOptionsSection _magdaOptions;
    private readonly ILogger<MagdaFacade> _logger;

    public MagdaFacade(
        MagdaOptionsSection magdaOptions,
        ILogger<MagdaFacade> logger)
    {
        _magdaOptions = magdaOptions;
        _logger = logger;
    }

    public async Task<ResponseEnvelope<GeefOndernemingVKBOResponseBody>?> GeefOndernemingVKBO(
        string kbonummer,
        MagdaCallReference reference)
    {
        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(_magdaOptions.GeefOndernemingVkboEndpoint, $"{nameof(MagdaOptionsSection.GeefOndernemingVkboEndpoint)}");

        var unsignedEnvelope = MakeEnvelope(GeefOndernemingVKBOBody.CreateRequest(kbonummer, reference.Reference, _magdaOptions));
        var clientCertificate = GetMagdaClientCertificate(_magdaOptions);
        var signedEnvelope = unsignedEnvelope.SignEnvelope(clientCertificate);

        return await PerformMagdaRequest<GeefOndernemingVKBOResponseBody>(
            _magdaOptions.GeefOndernemingVkboEndpoint!,
            clientCertificate,
            signedEnvelope);
    }

    public async Task<ResponseEnvelope<GeefOndernemingResponseBody>?> GeefOnderneming(string kbonummer, MagdaCallReference reference)
    {
        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(_magdaOptions.GeefOndernemingEndpoint, $"{nameof(MagdaOptionsSection.GeefOndernemingEndpoint)}");

        var unsignedEnvelope = MakeEnvelope(GeefOndernemingBody.CreateRequest(kbonummer, reference.Reference, _magdaOptions));
        var clientCertificate = GetMagdaClientCertificate(_magdaOptions);
        var signedEnvelope = unsignedEnvelope.SignEnvelope(clientCertificate);

        return await PerformMagdaRequest<GeefOndernemingResponseBody>(
            _magdaOptions.GeefOndernemingEndpoint!,
            clientCertificate,
            signedEnvelope);
    }

    private static MagdaClientCertificate? GetMagdaClientCertificate(MagdaOptionsSection magdaOptionsSection)
    {
        if (magdaOptionsSection.ClientCertificate is null && magdaOptionsSection.ClientCertificatePassword is null) return null;

        var maybeClientCertificate =
            MagdaClientCertificate.Create(magdaOptionsSection.ClientCertificate!, magdaOptionsSection.ClientCertificatePassword!);

        if (maybeClientCertificate is not { } clientCertificate)
            throw new NullReferenceException("ClientCertificate should never be null");

        return clientCertificate;
    }

    private async Task<ResponseEnvelope<T>?> PerformMagdaRequest<T>(
        string endpoint,
        X509Certificate? magdaClientCertificate,
        string signedEnvelope)
    {
        using var client = GetMagdaHttpClient(magdaClientCertificate);

        try
        {
            return await SendEnvelopeToendpoint<T>(endpoint, signedEnvelope, client);
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

    private HttpClient GetMagdaHttpClient(X509Certificate? magdaClientCertificate)
    {
        var client = new HttpClient(new MagdaHttpClientHandler(magdaClientCertificate));
        client.Timeout = TimeSpan.FromSeconds(_magdaOptions.Timeout);

        return client;
    }

    private async Task<ResponseEnvelope<T>?> SendEnvelopeToendpoint<T>(string endpoint, string signedEnvelope, HttpClient client)
    {
        var response = await client
           .PostAsync(
                endpoint,
                new StringContent(signedEnvelope, Encoding.UTF8, mediaType: "application/soap+xml"));

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(message: "GeefOnderneming response not successful: \n{@Result}\n{@Content}", response,
                               await response.Content.ReadAsStringAsync());

            return null;
        }

        _logger.LogTrace(message: "GeefOnderneming http response: {@Result}", response);

        var serializer = new XmlSerializer(typeof(ResponseEnvelope<T>));

        var xml = await response.Content.ReadAsStringAsync();
        using var reader = new StringReader(xml);

        {
            return (ResponseEnvelope<T>?)serializer.Deserialize(reader);
        }
    }

    private static Envelope<T> MakeEnvelope<T>(T body)
        => new()
        {
            Header = new Header(),
            Body = body,
        };
}
