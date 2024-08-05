namespace AssociationRegistry.Test.Admin.Api.Features.FeitelijkeVereniging.Steps;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using FluentAssertions.Execution;
using Framework;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx.Core;
using Marten;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Polly;
using System.Net;
using Vereniging;
using Adres = AssociationRegistry.Admin.Api.Verenigingen.Common.Adres;

public static class ContextExtensions
{
    public static void Add<T>(this DefaultTestRunContext context, T item)
        => context.Add(typeof(T).FullName, item);

    public static void Add<T>(this FeatureContext context, T item)
        => context.Add(typeof(T).FullName, item);

    public static void Add<T>(this ScenarioContext context, T item)
        => context.Add(typeof(T).FullName, item);
}

[Binding]
public class RegistreerStepDefinitions
{
    private readonly DefaultTestRunContext _testRunContext;
    private readonly FeatureContext _featureContext;
    private readonly ScenarioContext _scenarioContext;
    private readonly Fixture _fixture;

    public RegistreerStepDefinitions(
        DefaultTestRunContext testRunContext,
        FeatureContext featureContext,
        ScenarioContext scenarioContext)
    {
        _testRunContext = testRunContext;
        _featureContext = featureContext;
        _scenarioContext = scenarioContext;

        _fixture = testRunContext.Get<Fixture>();
    }

    [Given(@"Feitelijke vereniging werd geregistreerd met alle velden")]
    public void GivenFeitelijkeVerenigingWerdGeregistreerdMetAlleVelden()
    {
        _scenarioContext.Add(new RegistreerFeitelijkeVerenigingRequest
        {
            Naam = _fixture.Create<string>(),
            KorteNaam = _fixture.Create<string>(),
            KorteBeschrijving = _fixture.Create<string>(),
            Startdatum = DateOnly.FromDateTime(DateTime.Today),
            IsUitgeschrevenUitPubliekeDatastroom = true,
            Doelgroep = new DoelgroepRequest
            {
                Minimumleeftijd = 1,
                Maximumleeftijd = 149,
            },
            Contactgegevens = new ToeTeVoegenContactgegeven[]
            {
                new()
                {
                    Contactgegeventype = Contactgegeventype.Email,
                    Waarde = "random@example.org",
                    Beschrijving = "Algemeen",
                    IsPrimair = false,
                },
            },
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Naam = "Kantoor",
                    Adres = new Adres
                    {
                        Straatnaam = "Leopold II-laan",
                        Huisnummer = "99",
                        Busnummer = "",
                        Postcode = "9200",
                        Gemeente = "Dendermonde",
                        Land = "Belgie",
                    },
                    IsPrimair = true,
                    Locatietype = Locatietype.Correspondentie,
                },
                new ToeTeVoegenLocatie
                {
                    Naam = "Afhalingen",
                    Adres = new Adres
                    {
                        Straatnaam = "Leopold II-laan",
                        Huisnummer = "99",
                        Busnummer = "",
                        Postcode = "1234",
                        Gemeente = "Dendermonde",
                        Land = "Belgie",
                    },
                    IsPrimair = false,
                    Locatietype = Locatietype.Activiteiten,
                },
                new ToeTeVoegenLocatie
                {
                    Naam = "Speeltuin",
                    Adres = new Adres
                    {
                        Straatnaam = "dorpelstraat",
                        Huisnummer = "169",
                        Busnummer = "2",
                        Postcode = "4567",
                        Gemeente = "Nothingham",
                        Land = "Belgie",
                    },
                    IsPrimair = false,
                    Locatietype = Locatietype.Activiteiten,
                },
                new ToeTeVoegenLocatie
                {
                    Naam = "Zwembad",
                    Adres = new Adres
                    {
                        Straatnaam = "dorpelstraat",
                        Huisnummer = "169",
                        Busnummer = "2",
                        Postcode = "4567",
                        Gemeente = "Nothingham",
                        Land = "Belgie",
                    },
                    IsPrimair = false,
                    Locatietype = Locatietype.Activiteiten,
                },
            },
            Vertegenwoordigers = new[]
            {
                new ToeTeVoegenVertegenwoordiger
                {
                    Insz = _fixture.Create<Insz>(),
                    Voornaam = "Jane",
                    Achternaam = "Doe",
                    Rol = "Voorzitter, Hoofdcoach",
                    Roepnaam = "QTPY",
                    IsPrimair = true,
                    Email = "qtpy@example.org",
                    Telefoon = "0123456789",
                    Mobiel = "987654321",
                    SocialMedia = "http://example.com",
                },
                new ToeTeVoegenVertegenwoordiger
                {
                    Insz = _fixture.Create<Insz>(),
                    Voornaam = "Kim",
                    Achternaam = "Possible",
                    Rol = "Master",
                    Roepnaam = "Lara",
                    IsPrimair = false,
                    Email = "master@example.org",
                    Telefoon = "0000000000",
                    Mobiel = "6666666666",
                    SocialMedia = "http://example.com/scrum",
                },
            },
            HoofdactiviteitenVerenigingsloket = new[] { "BIAG", "BWWC" },
        });
    }

    [When(@"Registreer feitelijke vereniging endpoint has been called")]
    public async Task WhenRegistreerFeitelijkeVerenigingEndpointHasBeenCalled()
    {
        var request = _scenarioContext.Get<RegistreerFeitelijkeVerenigingRequest>();
        var response = await _featureContext.Get<AdminApiClient>().RegistreerFeitelijkeVereniging(GetJsonBody(request));
        _scenarioContext.Add(response);

        string GetJsonBody(RegistreerFeitelijkeVerenigingRequest req)
            => GetType()
              .GetAssociatedResourceJson("files.request.with_all_fields")
              .Replace(oldValue: "{{vereniging.naam}}", req.Naam)
              .Replace(oldValue: "{{vereniging.korteNaam}}", req.KorteNaam)
              .Replace(oldValue: "{{vereniging.korteBeschrijving}}", req.KorteBeschrijving)
              .Replace(oldValue: "{{vereniging.isUitgeschrevenUitPubliekeDatastroom}}",
                       req.IsUitgeschrevenUitPubliekeDatastroom.ToString().ToLower())
              .Replace(oldValue: "{{vereniging.startdatum}}", req.Startdatum!.Value.ToString(WellknownFormats.DateOnly))
              .Replace(oldValue: "{{vereniging.doelgroep.minimumleeftijd}}", req.Doelgroep!.Minimumleeftijd.ToString())
              .Replace(oldValue: "{{vereniging.doelgroep.maximumleeftijd}}", req.Doelgroep!.Maximumleeftijd.ToString())
              .Replace(oldValue: "{{vereniging.contactgegevens}}", JsonConvert.SerializeObject(req.Contactgegevens))
              .Replace(oldValue: "{{vereniging.locaties}}", JsonConvert.SerializeObject(req.Locaties))
              .Replace(oldValue: "{{vereniging.vertegenwoordigers}}", JsonConvert.SerializeObject(req.Vertegenwoordigers))
              .Replace(oldValue: "{{vereniging.hoofdactiviteitenLijst}}",
                       JsonConvert.SerializeObject(req.HoofdactiviteitenVerenigingsloket));
    }

    [Then(@"It saves the events")]
    public void ThenItSavesTheEvents()
    {
        using var session = _featureContext.Get<DocumentStore>().LightweightSession();

        var request = _scenarioContext.Get<RegistreerFeitelijkeVerenigingRequest>();

        var savedEvent = session.Events
                                .QueryRawEventDataOnly<FeitelijkeVerenigingWerdGeregistreerd>()
                                .Single(e => e.Naam == request.Naam);

        savedEvent.KorteNaam.Should().Be(request.KorteNaam);
        savedEvent.KorteBeschrijving.Should().Be(request.KorteBeschrijving);
        savedEvent.Startdatum.Should().Be(request.Startdatum!.Value);
        savedEvent.Contactgegevens.Should().HaveCount(expected: 1);
        savedEvent.Doelgroep.Should().BeEquivalentTo(request.Doelgroep);
        savedEvent.IsUitgeschrevenUitPubliekeDatastroom.Should().BeTrue();

        savedEvent.Contactgegevens[0].Should()
                  .BeEquivalentTo(request.Contactgegevens[0], config: options => options.ComparingEnumsByName());

        savedEvent.Locaties.Should().HaveCount(expected: 4);
        savedEvent.Locaties[0].Should().BeEquivalentTo(request.Locaties[0]);
        savedEvent.Locaties[1].Should().BeEquivalentTo(request.Locaties[1]);
        savedEvent.Locaties[2].Should().BeEquivalentTo(request.Locaties[2]);
        savedEvent.Locaties[3].Should().BeEquivalentTo(request.Locaties[3]);
        savedEvent.Locaties.ForEach(x => x.LocatieId.Should().BePositive());
        savedEvent.Locaties.Select(x => x.LocatieId).ToList().Should().OnlyHaveUniqueItems();

        savedEvent.Vertegenwoordigers.Should()
                  .BeEquivalentTo(request.Vertegenwoordigers, config: options => options.ComparingEnumsByName());

        savedEvent.Vertegenwoordigers.ForEach(x => x.VertegenwoordigerId.Should().BePositive());
        savedEvent.Vertegenwoordigers.Select(x => x.VertegenwoordigerId).ToList().Should().OnlyHaveUniqueItems();

        savedEvent.HoofdactiviteitenVerenigingsloket.Should().BeEquivalentTo(
            new[]
            {
                new Registratiedata.HoofdactiviteitVerenigingsloket(Code: "BIAG", Naam: "Burgerinitiatief & Actiegroep"),
                new Registratiedata.HoofdactiviteitVerenigingsloket(Code: "BWWC", Naam: "Buurtwerking & Wijkcomité"),
            });
    }

    [Then(@"It returns an accepted response")]
    public void ThenItReturnsAnAcceptedResponse()
    {
        var response = _scenarioContext.Get<HttpResponseMessage>();
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Then(@"It returns a location header")]
    public void ThenItReturnsALocationHeader(DefaultTestRunContext testRunContext, ScenarioContext scenarioContext)
    {
        var response = scenarioContext.Get<HttpResponseMessage>();

        response.Headers.Should().ContainKey(HeaderNames.Location);

        response.Headers.Location!.OriginalString.Should()
                .StartWith($"{testRunContext.Get<AppSettings>().BaseUrl}/v1/verenigingen/V");
    }

    [Then(@"It returns a sequence header")]
    public void ThenItReturnsASequenceHeader()
    {
        var response = _scenarioContext.Get<HttpResponseMessage>();

        response.Headers.Should().ContainKey(WellknownHeaderNames.Sequence);
        var sequenceValues = response.Headers.GetValues(WellknownHeaderNames.Sequence).ToList();
        sequenceValues.Should().HaveCount(expected: 1);
        var sequence = Convert.ToInt64(sequenceValues.Single());
        sequence.Should().BeGreaterThan(expected: 0);
    }

    [Then(@"It should have place a message on sqs for address match")]
    public async Task ThenItShouldHavePlaceAMessageOnSqsForAddressMatch()
    {
        await using var session = _testRunContext.Get<DocumentStore>().LightweightSession();

        var request = _scenarioContext.Get<FeitelijkeVerenigingWerdGeregistreerd>();
        var response = _scenarioContext.Get<HttpResponseMessage>();
        response.Should().NotBeNull();

        var savedEvent = await session.Events
                                      .QueryRawEventDataOnly<FeitelijkeVerenigingWerdGeregistreerd>()
                                      .SingleOrDefaultAsync(e => e.Naam == request.Naam);

        var policyResult = await Policy.Handle<Exception>()
                                       .RetryAsync(retryCount: 5, onRetryAsync: async (_, i) => await Task.Delay(TimeSpan.FromSeconds(i)))
                                       .ExecuteAndCaptureAsync(async () =>
                                        {
                                            await using var retrySession = _testRunContext.Get<DocumentStore>().LightweightSession();

                                            var werdOvergenomen =
                                                retrySession.SingleOrDefaultFromStream<AdresWerdOvergenomenUitAdressenregister>(
                                                    savedEvent.VCode);

                                            using (new AssertionScope())
                                            {
                                                werdOvergenomen.Should().NotBeNull();

                                                werdOvergenomen.AdresId.Should().BeEquivalentTo(
                                                    new Registratiedata.AdresId(
                                                        Broncode: "AR", Bronwaarde: "https://data.vlaanderen.be/id/adres/3213019"));

                                                retrySession.SingleOrDefaultFromStream<AdresNietUniekInAdressenregister>(savedEvent.VCode)
                                                            .Should().NotBeNull();
                                            }
                                        });

        policyResult.FinalException.Should().BeNull();
    }
}
