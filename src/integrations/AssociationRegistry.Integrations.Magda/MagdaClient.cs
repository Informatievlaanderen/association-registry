namespace AssociationRegistry.Integrations.Magda;

using AssociationRegistry.Magda.Kbo;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Framework;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.Extensions.Logging;
using Onderneming;
using Onderneming.Models;
using Onderneming.Models.GeefOnderneming;
using Onderneming.Models.RegistreerInschrijving;
using Persoon.Models;
using Persoon.Models.RegistreerInschrijving0200;
using Persoon.Models.RegistreerUitschrijving;
using Persoon.Validation;
using Shared.Exceptions;
using Shared.Extensions;
using Shared.Models;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using RegistreerInschrijvingResponseBody = Onderneming.Models.RegistreerInschrijving.RegistreerInschrijvingResponseBody;

public class MagdaClient : IMagdaClient
{
    private readonly MagdaOptionsSection _magdaOptions;
    private readonly IMagdaCallReferenceService _magdaReferenceService;
    private readonly IMagdaRegistreerInschrijvingValidator _registreerInschrijvingValidator;
    private readonly IMagdaGeefPersoonValidator _geefPersoonValidator;
    private readonly ILogger<MagdaClient> _logger;

    public MagdaClient(
        MagdaOptionsSection magdaOptions,
        IMagdaCallReferenceService magdaReferenceService,
        IMagdaRegistreerInschrijvingValidator registreerInschrijvingValidator,
        IMagdaGeefPersoonValidator geefPersoonValidator,
        ILogger<MagdaClient> logger)
    {
        _magdaOptions = magdaOptions;
        _magdaReferenceService = magdaReferenceService;
        _registreerInschrijvingValidator = registreerInschrijvingValidator;
        _geefPersoonValidator = geefPersoonValidator;
        _logger = logger;
    }


    public async Task<ResponseEnvelope<GeefOndernemingResponseBody>?> GeefOnderneming(string kboNummer, AanroependeFunctie aanroependeFunctie, CommandMetadata metadata, CancellationToken cancellationToken)
    {
        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(_magdaOptions.GeefOndernemingEndpoint, $"{nameof(MagdaOptionsSection.GeefOndernemingEndpoint)}");

        var reference = await _magdaReferenceService.CreateReference(metadata.Initiator,
                                                                     metadata.CorrelationId,
                                                                     kboNummer,
                                                                     ReferenceContext.GeefOndernemingDienst0200(aanroependeFunctie),
                                                                     cancellationToken);

        _logger.LogInformation($"MAGDA Call Reference - GeefOnderneming - KBO nummer '{kboNummer}' met referentie '{reference.Reference}'");

        var unsignedEnvelope = MakeEnvelope(GeefOndernemingBody.CreateRequest(kboNummer, reference.Reference, _magdaOptions));
        var clientCertificate = GetMagdaClientCertificate(_magdaOptions);
        var signedEnvelope = unsignedEnvelope.SignEnvelope(clientCertificate);

        var xml = await PerformMagdaRequest<GeefOndernemingResponseBody>(
            _magdaOptions.GeefOndernemingEndpoint!,
            clientCertificate,
            signedEnvelope);

        return LogXmlForUnexpectedException(() => DeserializeResponse<GeefOndernemingResponseBody>(xml), xml);
    }

    // TODO: change to registreerUitschrijvingPersoon
    public async Task<ResponseEnvelope<RegistreerUitschrijvingResponseBody>?> RegistreerUitschrijving(
        string kbonummer,
        AanroependeFunctie aanroependeFunctie,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
    {
        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(_magdaOptions.RegistreerUitschrijvingEndpoint, $"{nameof(MagdaOptionsSection.RegistreerUitschrijvingEndpoint)}");

        var reference = await _magdaReferenceService.CreateReference(metadata.Initiator,
                                                                     metadata.CorrelationId,
                                                                     kbonummer,
                                                                     ReferenceContext.RegistreerUitschrijving0201(aanroependeFunctie),
                                                                     cancellationToken);
        _logger.LogInformation(
            $"MAGDA Call Reference - RegistreerUitschrijving - KBO nummer '{kbonummer}' met referentie '{reference.Reference}'");

        var unsignedEnvelope = MakeEnvelope(RegistreerUitschrijvingBody.CreateRequest(kbonummer, reference.Reference, _magdaOptions));
        var clientCertificate = GetMagdaClientCertificate(_magdaOptions);
        var signedEnvelope = unsignedEnvelope.SignEnvelope(clientCertificate);

        var xml = await PerformMagdaRequest<RegistreerUitschrijvingResponseBody>(
            _magdaOptions.RegistreerUitschrijvingEndpoint!,
            clientCertificate,
            signedEnvelope);

        return LogXmlForUnexpectedException(() => DeserializeResponse<RegistreerUitschrijvingResponseBody>(xml), xml);

    }

    public async Task<ResponseEnvelope<RegistreerInschrijvingResponseBody>?> RegistreerInschrijvingOnderneming(
        string kbonummer,
        AanroependeFunctie aanroependeFunctie,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
    {
        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(_magdaOptions.RegistreerInschrijvingEndpoint,
                               $"{nameof(MagdaOptionsSection.RegistreerInschrijvingEndpoint)}");

        var reference = await _magdaReferenceService.CreateReference(metadata.Initiator,
                                                                     metadata.CorrelationId,
                                                                     kbonummer,
                                                                     ReferenceContext.RegistreerInschrijving0201(aanroependeFunctie),
                                                                     cancellationToken);
        _logger.LogInformation(
            $"MAGDA Call Reference - RegistreerInschrijving - KBO nummer '{kbonummer}' met referentie '{reference.Reference}'");

        var unsignedEnvelope = MakeEnvelope(RegistreerInschrijvingBody.CreateRequest(kbonummer, reference.Reference, _magdaOptions));
        var clientCertificate = GetMagdaClientCertificate(_magdaOptions);
        var signedEnvelope = unsignedEnvelope.SignEnvelope(clientCertificate);

        var xml = await PerformMagdaRequest<RegistreerInschrijvingResponseBody>(
            _magdaOptions.RegistreerInschrijvingEndpoint!,
            clientCertificate,
            signedEnvelope);

        return LogXmlForUnexpectedException(() => DeserializeResponse<RegistreerInschrijvingResponseBody>(xml), xml);
    }

    public async Task<ResponseEnvelope<Persoon.Models.RegistreerInschrijving0200.RegistreerInschrijvingResponseBody>?> RegistreerInschrijvingPersoon(
        string insz,
        AanroependeFunctie aanroependeFunctie,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
    {
        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(_magdaOptions.RegistreerInschrijvingPersoonEndpoint,
                               $"{nameof(MagdaOptionsSection.RegistreerInschrijvingPersoonEndpoint)}");

        var reference = await _magdaReferenceService.CreateReference(metadata.Initiator,
                                                                     metadata.CorrelationId,
                                                                     insz,
                                                                     ReferenceContext.RegistreerInschrijving0200(aanroependeFunctie),
                                                                     cancellationToken);

        _logger.LogInformation(
            $"MAGDA Call Reference - RegistreerInschrijving Persoon met referentie '{reference.Reference}'");

        var unsignedEnvelope = MakeEnvelope(RegistreerInschrijvingPersoonBody.CreateRequest(insz, reference.Reference, _magdaOptions));
        var clientCertificate = GetMagdaClientCertificate(_magdaOptions);
        var signedEnvelope = unsignedEnvelope.SignEnvelope(clientCertificate);

        var xml = await PerformMagdaRequest<Persoon.Models.RegistreerInschrijving0200.RegistreerInschrijvingResponseBody>(
            _magdaOptions.RegistreerInschrijvingPersoonEndpoint!,
            clientCertificate,
            signedEnvelope);

        return LogXmlForUnexpectedException(() =>
        {
            var response = DeserializeResponse<Persoon.Models.RegistreerInschrijving0200.RegistreerInschrijvingResponseBody>(xml);
            _registreerInschrijvingValidator.ValidateOrThrow(response, metadata.CorrelationId);

            return response;
        }, xml);
    }

    private ResponseEnvelope<TResponseBody> LogXmlForUnexpectedException<TResponseBody>(Func<ResponseEnvelope<TResponseBody>> action, string xml)
    {
        try
        {
            return action();
        }
        catch (DomainException e)
        {
            throw;
        }
        catch (MagdaException e)
        {
            throw;
        }
        catch (Exception e)
        {

            _logger.LogError("Onverwachte fout bij RegistreerInschrijving met xml structuur: \n" + GetStructure(XDocument.Parse(xml).Root));

            throw;
        }
    }

    private ResponseEnvelope<T> DeserializeResponse<T>(string response)
    {
        var serializer = new XmlSerializer(typeof(ResponseEnvelope<T>));

        try
        {
            using var reader = new StringReader(response);

            return (ResponseEnvelope<T>?)serializer.Deserialize(reader) ?? throw new MagdaException($"Could not Serialize response");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);

            throw;
        }
    }

    public static XElement GetStructure(XElement element)
    {
        return new XElement(element.Name,
                            element.Attributes().Select(a => new XAttribute(a.Name, "•")),
                            element.Elements().Select(GetStructure)
        );
    }

    public async Task<ResponseEnvelope<GeefPersoonResponseBody>> GeefPersoon(string insz, AanroependeFunctie functie, CommandMetadata metadata, CancellationToken cancellationToken)
    {
        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(_magdaOptions.GeefPersoonEndpoint,
                               $"{nameof(MagdaOptionsSection.GeefPersoonEndpoint)}");

        var reference = await _magdaReferenceService.CreateReference(metadata.Initiator,
                                                                     metadata.CorrelationId,
                                                                     insz,
                                                                     ReferenceContext.GeefPersoon0200(functie),
                                                                     cancellationToken);

        _logger.LogInformation(
            $"MAGDA Call Reference - GeefPersoon met referentie '{reference.Reference}'");

        var unsignedEnvelope = MakeEnvelope(GeefPersoonBody.CreateRequest(insz, reference.Reference, _magdaOptions));
        var clientCertificate = GetMagdaClientCertificate(_magdaOptions);
        var signedEnvelope = unsignedEnvelope.SignEnvelope(clientCertificate);

        var xml = await PerformMagdaRequest<GeefPersoonResponseBody>(
            _magdaOptions.GeefPersoonEndpoint!,
            clientCertificate,
            signedEnvelope);

        return LogXmlForUnexpectedException(() =>
        {
            var response = DeserializeResponse<GeefPersoonResponseBody>(xml);
            _geefPersoonValidator.ValidateOrThrow(response, metadata.CorrelationId);

            return response;
        }, xml);
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

    private async Task<string> PerformMagdaRequest<T>(
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

    private async Task<string> SendEnvelopeToendpoint<T>(string endpoint, string signedEnvelope, HttpClient client)
    {
        var response = await client
           .PostAsync(
                endpoint,
                new StringContent(signedEnvelope, Encoding.UTF8, mediaType: "application/soap+xml"));

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogWarning(message: "Magda returned non success status code \n{@Result}\n{@Content}", response,
                               content);

            throw MagdaException.WithNonSuccessStatus(response, content);
        }

        _logger.LogTrace(message: "Magda call http response: {@Result}", response);


        var xml = await response.Content.ReadAsStringAsync();

        return xml;
    }

    private static Envelope<T> MakeEnvelope<T>(T body)
        => new()
        {
            Header = new Header(),
            Body = body,
        };
}

public record ReferenceContext(string MagdaDienst, AanroependeFunctie AanroependeFunctie)
{
    public static ReferenceContext GeefPersoon0200(AanroependeFunctie functie) => new("GeefPersoonDienst-02.00", functie);
    public static ReferenceContext RegistreerInschrijving0200(AanroependeFunctie functie) => new("RegistreerInschrijvingDienst-02.00", functie);
    public static ReferenceContext RegistreerUitschrijving0200(AanroependeFunctie functie) => new("RegistreerUitschrijvingDienst-02.00", functie);
    public static ReferenceContext RegistreerInschrijving0201(AanroependeFunctie functie) => new("RegistreerInschrijvingDienst-02.01", functie);
    public static ReferenceContext RegistreerUitschrijving0201(AanroependeFunctie functie) => new("RegistreerUitschrijvingDienst-02.01", functie);
    public static ReferenceContext GeefOndernemingDienst0200(AanroependeFunctie functie) => new("GeefOndernemingDienst-02.00", functie);
};


