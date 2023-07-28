namespace AssociationRegistry.Magda;

using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;
using Configuration;
using Microsoft.Extensions.Logging;
using Models;
using Onderneming.GeefOndernemingVKBO;

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

    public async Task<ResponseEnvelope<GeefOndernemingResponseBody>?> GeefOnderneming(string kbonummer, MagdaCallReference reference)
    {
        var unsignedEnvelope = MakeEnvelope(GeefOndernemingBodyWith(kbonummer, reference.Reference, _magdaOptions));
        var clientCertificate = GetMagdaClientCertificate(_magdaOptions);
        var signedEnvelope = unsignedEnvelope.SignEnvelope(clientCertificate);

        return await PerformMagdaRequest<GeefOndernemingResponseBody>(
            _magdaOptions.GeefOndernemingVkboEndpoint,
            clientCertificate,
            signedEnvelope);
    }

    private static MagdaClientCertificate? GetMagdaClientCertificate(MagdaOptionsSection magdaOptionsSection)
    {
        if (magdaOptionsSection.ClientCertificate is null && magdaOptionsSection.ClientCertificatePassword is null) return null;

        var maybeClientCertificate = MagdaClientCertificate.Create(magdaOptionsSection.ClientCertificate!, magdaOptionsSection.ClientCertificatePassword!);

        if (maybeClientCertificate is not { } clientCertificate)
            throw new NullReferenceException("ClientCertificate should never be null");

        return clientCertificate;
    }

    private async Task<ResponseEnvelope<T>?> PerformMagdaRequest<T>(string endpoint, X509Certificate? magdaClientCertificate, string signedEnvelope)
    {
        using var client = GetMagdaHttpClient(magdaClientCertificate);
        try
        {
            return await SendEnvelopeToendpoint<T>(endpoint, signedEnvelope, client);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            throw new Exception("A timeout occurred when calling the Magda services", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
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
                new StringContent(signedEnvelope, Encoding.UTF8, "application/soap+xml"));

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("GeefOnderneming response not successful: {@Result}", response);
            return null;
        }

        _logger.LogTrace("GeefOnderneming http response: {@Result}", response);

        var serializer = new XmlSerializer(typeof(ResponseEnvelope<T>));

        var xml = await response.Content.ReadAsStringAsync();
        using var reader = new StringReader(xml);
        {
            return (ResponseEnvelope<T>?)serializer.Deserialize(reader);
        }
    }

    private static GeefOndernemingBody GeefOndernemingBodyWith(string kboNummer, Guid reference, MagdaOptionsSection magdaOptionsSection)
        => new()
        {
            GeefOnderneming = new GeefOndernemingVKBO
            {
                Verzoek = new VerzoekType
                {
                    Context = new ContextType
                    {
                        Naam = "GeefOndernemingVKBO",
                        Versie = "02.00.0000",
                        Bericht = new BerichtType
                        {
                            Type = BerichtTypeType.VRAAG,
                            Tijdstip = new TijdstipType
                            {
                                Datum = DateTime.Now.ToString("yyyy-MM-dd"),
                                Tijd = DateTime.Now.ToString("HH:mm:ss.000"),
                            },
                            Afzender = new AfzenderAdresType
                            {
                                Identificatie = magdaOptionsSection.Afzender,
                                Referte = reference.ToString(),
                            },
                            Ontvanger = new OntvangerAdresType
                            {
                                Identificatie = magdaOptionsSection.Ontvanger,
                            },
                        },
                    },
                    Vragen = new VragenType
                    {
                        Vraag = new VraagType
                        {
                            Referte = reference.ToString(),
                            Inhoud = new VraagInhoudType
                            {
                                Ondernemingsnummer = kboNummer,
                            },
                        },
                    },
                },
            },
        };

    private static Envelope<T> MakeEnvelope<T>(T body)
        => new()
        {
            Header = new Header(),
            Body = body,
        };
}
