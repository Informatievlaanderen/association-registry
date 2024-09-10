namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using FluentAssertions;
using FluentAssertions.Execution;
using Formats;
using Framework;
using Framework.Fixtures;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Polly;
using System.Net;
using Vereniging;
using Xunit;
using Xunit.Categories;
using Adres = AssociationRegistry.Admin.Api.Verenigingen.Common.Adres;

public sealed class When_RegistreerFeitelijkeVereniging_WithAllFields
{
    private static When_RegistreerFeitelijkeVereniging_WithAllFields? called;
    public readonly RegistreerFeitelijkeVerenigingRequest Request;
    public readonly HttpResponseMessage Response;

    private When_RegistreerFeitelijkeVereniging_WithAllFields(AdminApiFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        Request = new RegistreerFeitelijkeVerenigingRequest
        {
            Naam = autoFixture.Create<string>(),
            KorteNaam = autoFixture.Create<string>(),
            KorteBeschrijving = autoFixture.Create<string>(),
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
                        Land = "België",
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
                        Land = "België",
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
                        Land = "België",
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
                        Land = "België",
                    },
                    IsPrimair = false,
                    Locatietype = Locatietype.Activiteiten,
                },
            },
            Vertegenwoordigers = new[]
            {
                new ToeTeVoegenVertegenwoordiger
                {
                    Insz = autoFixture.Create<Insz>(),
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
                    Insz = autoFixture.Create<Insz>(),
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
        };

        Response ??= fixture.DefaultClient.RegistreerFeitelijkeVereniging(GetJsonBody(Request)).GetAwaiter().GetResult();

        using var session = fixture.DocumentStore.LightweightSession();
    }

    public static When_RegistreerFeitelijkeVereniging_WithAllFields Called(AdminApiFixture fixture)
        => called ??= new When_RegistreerFeitelijkeVereniging_WithAllFields(fixture);

    private string GetJsonBody(RegistreerFeitelijkeVerenigingRequest request)
        => GetType()
          .GetAssociatedResourceJson("files.request.with_all_fields")
          .Replace(oldValue: "{{vereniging.naam}}", request.Naam)
          .Replace(oldValue: "{{vereniging.korteNaam}}", request.KorteNaam)
          .Replace(oldValue: "{{vereniging.korteBeschrijving}}", request.KorteBeschrijving)
          .Replace(oldValue: "{{vereniging.isUitgeschrevenUitPubliekeDatastroom}}",
                   request.IsUitgeschrevenUitPubliekeDatastroom.ToString().ToLower())
          .Replace(oldValue: "{{vereniging.startdatum}}", request.Startdatum!.Value.ToString(WellknownFormats.DateOnly))
          .Replace(oldValue: "{{vereniging.doelgroep.minimumleeftijd}}", request.Doelgroep!.Minimumleeftijd.ToString())
          .Replace(oldValue: "{{vereniging.doelgroep.maximumleeftijd}}", request.Doelgroep!.Maximumleeftijd.ToString())
          .Replace(oldValue: "{{vereniging.contactgegevens}}", JsonConvert.SerializeObject(request.Contactgegevens))
          .Replace(oldValue: "{{vereniging.locaties}}", JsonConvert.SerializeObject(request.Locaties))
          .Replace(oldValue: "{{vereniging.vertegenwoordigers}}", JsonConvert.SerializeObject(request.Vertegenwoordigers))
          .Replace(oldValue: "{{vereniging.hoofdactiviteitenLijst}}",
                   JsonConvert.SerializeObject(request.HoofdactiviteitenVerenigingsloket));
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_All_Fields
{
    private readonly EventsInDbScenariosFixture _fixture;

    public With_All_Fields(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    private RegistreerFeitelijkeVerenigingRequest Request
        => When_RegistreerFeitelijkeVereniging_WithAllFields.Called(_fixture).Request;

    private HttpResponseMessage Response
        => When_RegistreerFeitelijkeVereniging_WithAllFields.Called(_fixture).Response;

    [Fact]
    [Obsolete("Move to unit test")]
    public void Then_it_saves_the_events()
    {
        using var session = _fixture.DocumentStore
                                    .LightweightSession();

        var savedEvent = session.Events
                                .QueryRawEventDataOnly<FeitelijkeVerenigingWerdGeregistreerd>()
                                .Single(e => e.Naam == Request.Naam);

        savedEvent.KorteNaam.Should().Be(Request.KorteNaam);
        savedEvent.KorteBeschrijving.Should().Be(Request.KorteBeschrijving);
        savedEvent.Startdatum.Should().Be(Request.Startdatum!.Value);
        savedEvent.Contactgegevens.Should().HaveCount(expected: 1);
        savedEvent.Doelgroep.Should().BeEquivalentTo(Request.Doelgroep);
        savedEvent.IsUitgeschrevenUitPubliekeDatastroom.Should().BeTrue();

        savedEvent.Contactgegevens[0].Should()
                  .BeEquivalentTo(Request.Contactgegevens[0], config: options => options.ComparingEnumsByName());

        savedEvent.Locaties.Should().HaveCount(expected: 4);
        savedEvent.Locaties[0].Should().BeEquivalentTo(Request.Locaties[0]);
        savedEvent.Locaties[1].Should().BeEquivalentTo(Request.Locaties[1]);
        savedEvent.Locaties[2].Should().BeEquivalentTo(Request.Locaties[2]);
        savedEvent.Locaties[3].Should().BeEquivalentTo(Request.Locaties[3]);
        savedEvent.Locaties.ForEach(x => x.LocatieId.Should().BePositive());
        savedEvent.Locaties.Select(x => x.LocatieId).ToList().Should().OnlyHaveUniqueItems();

        savedEvent.Vertegenwoordigers.Should()
                  .BeEquivalentTo(Request.Vertegenwoordigers, config: options => options.ComparingEnumsByName());

        savedEvent.Vertegenwoordigers.ForEach(x => x.VertegenwoordigerId.Should().BePositive());
        savedEvent.Vertegenwoordigers.Select(x => x.VertegenwoordigerId).ToList().Should().OnlyHaveUniqueItems();

        savedEvent.HoofdactiviteitenVerenigingsloket.Should().BeEquivalentTo(
            new[]
            {
                new Registratiedata.HoofdactiviteitVerenigingsloket(Code: "BIAG", Naam: "Burgerinitiatief & Actiegroep"),
                new Registratiedata.HoofdactiviteitVerenigingsloket(Code: "BWWC", Naam: "Buurtwerking & Wijkcomité"),
            });
    }

    [Fact]
    [Obsolete("Move to unit test")]
    public async Task Then_it_should_have_placed_message_on_sqs_for_address_match()
    {
        var savedEvent = GetFeitelijkeVerenigingWerdGeregistreerdEvent();

        Response.Should().NotBeNull();

        var policyResult = await Policy.Handle<Exception>()
                                       .RetryAsync(retryCount: 5, onRetryAsync: async (_, i) => await Task.Delay(TimeSpan.FromSeconds(i)))
                                       .ExecuteAndCaptureAsync(async () =>
                                        {
                                            await using var session = _fixture.DocumentStore.LightweightSession();

                                            var werdOvergenomen =
                                                session.SingleOrDefaultFromStream<AdresWerdOvergenomenUitAdressenregister>(
                                                    savedEvent.VCode);

                                            using (new AssertionScope())
                                            {
                                                werdOvergenomen.Should().NotBeNull();

                                                werdOvergenomen.AdresId.Should().BeEquivalentTo(
                                                    new Registratiedata.AdresId(
                                                        Broncode: "AR", Bronwaarde: "https://data.vlaanderen.be/id/adres/3213019"));

                                                session.SingleOrDefaultFromStream<AdresNietUniekInAdressenregister>(savedEvent.VCode)
                                                       .Should().NotBeNull();
                                            }
                                        });

        policyResult.FinalException.Should().BeNull();
    }

    private FeitelijkeVerenigingWerdGeregistreerd? GetFeitelijkeVerenigingWerdGeregistreerdEvent()
    {
        using var session = _fixture.DocumentStore
                                    .LightweightSession();

        return session.Events
                      .QueryRawEventDataOnly<FeitelijkeVerenigingWerdGeregistreerd>()
                      .SingleOrDefault(e => e.Naam == Request.Naam);
    }
}
