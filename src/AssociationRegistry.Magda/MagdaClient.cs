namespace AssociationRegistry.Magda;

using Extensions;
using Framework;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Models;
using Models.GeefOnderneming;
using Models.GeefOndernemingVKBO;
using Models.RegistreerInschrijving;
using Models.RegistreerUitschrijving;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;

public class MagdaClient : IMagdaClient
{
    private readonly MagdaOptionsSection _magdaOptions;
    private readonly ILogger<MagdaClient> _logger;

    public MagdaClient(
        MagdaOptionsSection magdaOptions,
        ILogger<MagdaClient> logger)
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

        _logger.LogInformation($"MAGDA Call Reference - GeefOndernemingVKBO - KBO nummer '{kbonummer}' met referentie '{reference.Reference}'");

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

        _logger.LogInformation($"MAGDA Call Reference - GeefOnderneming - KBO nummer '{kbonummer}' met referentie '{reference.Reference}'");

        var unsignedEnvelope = MakeEnvelope(GeefOndernemingBody.CreateRequest(kbonummer, reference.Reference, _magdaOptions));
        var clientCertificate = GetMagdaClientCertificate(_magdaOptions);
        var signedEnvelope = unsignedEnvelope.SignEnvelope(clientCertificate);

        return await PerformMagdaRequest<GeefOndernemingResponseBody>(
            _magdaOptions.GeefOndernemingEndpoint!,
            clientCertificate,
            signedEnvelope);
    }

    public async Task<ResponseEnvelope<RegistreerUitschrijvingResponseBody>?> RegistreerUitschrijving(
        string kbonummer,
        MagdaCallReference reference)
    {
        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(_magdaOptions.GeefOndernemingVkboEndpoint, $"{nameof(MagdaOptionsSection.GeefOndernemingVkboEndpoint)}");

        _logger.LogInformation($"MAGDA Call Reference - RegistreerUitschrijving - KBO nummer '{kbonummer}' met referentie '{reference.Reference}'");

        var unsignedEnvelope = MakeEnvelope(RegistreerUitschrijvingBody.CreateRequest(kbonummer, reference.Reference, _magdaOptions));
        var clientCertificate = GetMagdaClientCertificate(_magdaOptions);
        var signedEnvelope = unsignedEnvelope.SignEnvelope(clientCertificate);

        return await PerformMagdaRequest<RegistreerUitschrijvingResponseBody>(
            _magdaOptions.RegistreerUitschrijvingEndpoint!,
            clientCertificate,
            signedEnvelope);
    }

    public async Task<ResponseEnvelope<RegistreerInschrijvingResponseBody>?> RegistreerInschrijving(
        string kbonummer,
        MagdaCallReference reference)
    {
        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(_magdaOptions.RegistreerInschrijvingEndpoint,
                               $"{nameof(MagdaOptionsSection.RegistreerInschrijvingEndpoint)}");

        _logger.LogInformation($"MAGDA Call Reference - RegistreerInschrijving - KBO nummer '{kbonummer}' met referentie '{reference.Reference}'");

        var unsignedEnvelope = MakeEnvelope(RegistreerInschrijvingBody.CreateRequest(kbonummer, reference.Reference, _magdaOptions));
        _logger.LogInformation("MagdaClient.MagdaClient -> after MakeEnvelope");
        if (unsignedEnvelope == null)
            _logger.LogInformation("unsignedEnvelope is null");

        var clientCertificate = GetMagdaClientCertificate(_magdaOptions);
        _logger.LogInformation("MagdaClient.MagdaClient -> after GetMagdaClientCertificate");
        if (clientCertificate == null)
            _logger.LogInformation("clientCertificate is null");

        var signedEnvelope = unsignedEnvelope.SignEnvelope(clientCertificate);
        _logger.LogInformation("MagdaClient.MagdaClient -> after SignEnvelope");
        if (clientCertificate == null)
            _logger.LogInformation("signedEnvelope is null");

        if (_magdaOptions == null)
            _logger.LogInformation("_magdaOptions is null");


        return await PerformMagdaRequest<RegistreerInschrijvingResponseBody>(
            _magdaOptions.RegistreerInschrijvingEndpoint!,
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
        _logger.LogInformation("MagdaClient.PerformMagdaRequest");

        try
        {
            using var client = GetMagdaHttpClient(magdaClientCertificate);
            _logger.LogInformation("Client created");

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
        _logger.LogInformation("MagdaClient -> SendEnvelopeToendpoint");

        HttpResponseMessage response;
        try
        {
           response = await client
               .PostAsync(
                    endpoint,
                    new StringContent(signedEnvelope, Encoding.UTF8, mediaType: "application/soap+xml"));

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while executing a POST on MagdaClient -> SendEnvelopeToendpoint");

            throw;
        }

        _logger.LogInformation("Client returned statuscode: {STATUSCODE}", response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(message: "Magda call not successful: \n{@Result}\n{@Content}", response,
                               await response.Content.ReadAsStringAsync());

            return null;
        }

        _logger.LogTrace(message: "Magda call http response: {@Result}", response);

        var serializer = new XmlSerializer(typeof(ResponseEnvelope<T>));

        var xml = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("Reading xml response as string");

        try
        {
            using var reader = new StringReader(xml);

            {
                return (ResponseEnvelope<T>?)serializer.Deserialize(reader);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Fout bij serializen van xml");

            throw;
        }
    }

    private static Envelope<T> MakeEnvelope<T>(T body)
        => new()
        {
            Header = new Header(),
            Body = body,
        };
}
