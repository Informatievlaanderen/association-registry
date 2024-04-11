﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using FluentAssertions.Execution;
using Framework;
using JasperFx.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Polly;
using System.Net;
using Vereniging;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using Adres = AssociationRegistry.Admin.Api.Verenigingen.Common.Adres;

public sealed class When_RegistreerFeitelijkeVereniging_WithAllFields_And_PostalInformation
{
    private static When_RegistreerFeitelijkeVereniging_WithAllFields_And_PostalInformation? called;
    public readonly RegistreerFeitelijkeVerenigingRequest Request;
    public readonly HttpResponseMessage Response;

    private When_RegistreerFeitelijkeVereniging_WithAllFields_And_PostalInformation(AdminApiFixture fixture)
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
                        Straatnaam = "Fosselstraat",
                        Huisnummer = "48",
                        Busnummer = "",
                        Postcode = "1790",
                        Gemeente = "Affligem",
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
                        Straatnaam = "Fosselstraat",
                        Huisnummer = "48",
                        Busnummer = "",
                        Postcode = "1790",
                        Gemeente = "Hekene",
                        Land = "Belgie",
                    },
                    IsPrimair = false,
                    Locatietype = Locatietype.Activiteiten,
                },
                new ToeTeVoegenLocatie
                {
                    Naam = "Afhalingen",
                    Adres = new Adres
                    {
                        Straatnaam = "Fosselstraat",
                        Huisnummer = "48",
                        Busnummer = "",
                        Postcode = "1790",
                        Gemeente = "Nothingham",
                        Land = "Belgie",
                    },
                    IsPrimair = false,
                    Locatietype = Locatietype.Activiteiten,
                }
            }
        };

        Response ??= fixture.DefaultClient.RegistreerFeitelijkeVereniging(GetJsonBody(Request)).GetAwaiter().GetResult();

        using var session = fixture.DocumentStore.LightweightSession();
    }

    public static When_RegistreerFeitelijkeVereniging_WithAllFields_And_PostalInformation Called(AdminApiFixture fixture)
        => called ??= new When_RegistreerFeitelijkeVereniging_WithAllFields_And_PostalInformation(fixture);

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
public class With_All_Fields_And_PostalInformation
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public With_All_Fields_And_PostalInformation(EventsInDbScenariosFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _fixture = fixture;
        _testOutputHelper = testOutputHelper;
    }

    private RegistreerFeitelijkeVerenigingRequest Request
        => When_RegistreerFeitelijkeVereniging_WithAllFields_And_PostalInformation.Called(_fixture).Request;

    private HttpResponseMessage Response
        => When_RegistreerFeitelijkeVereniging_WithAllFields_And_PostalInformation.Called(_fixture).Response;

    [Fact]
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

        savedEvent.Locaties.Should().HaveCount(expected: 3);
        savedEvent.Locaties[0].Should().BeEquivalentTo(Request.Locaties[0]);
        savedEvent.Locaties[1].Should().BeEquivalentTo(Request.Locaties[1]);
        savedEvent.Locaties[2].Should().BeEquivalentTo(Request.Locaties[2]);
        savedEvent.Locaties.ForEach(x => x.LocatieId.Should().BePositive());
        savedEvent.Locaties.Select(x => x.LocatieId).ToList().Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public void Then_it_returns_a_location_header()
    {
        Response.Headers.Should().ContainKey(HeaderNames.Location);

        Response.Headers.Location!.OriginalString.Should()
                .StartWith($"{_fixture.ServiceProvider.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");
    }

    [Fact]
    public void Then_it_returns_a_sequence_header()
    {
        Response.Headers.Should().ContainKey(WellknownHeaderNames.Sequence);
        var sequenceValues = Response.Headers.GetValues(WellknownHeaderNames.Sequence).ToList();
        sequenceValues.Should().HaveCount(expected: 1);
        var sequence = Convert.ToInt64(sequenceValues.Single());
        sequence.Should().BeGreaterThan(expected: 0);
    }

    [Fact]
    public async Task Then_it_should_have_placed_message_on_sqs_for_address_match()
    {
        var savedEvent = GetFeitelijkeVerenigingWerdGeregistreerdEvent();

        _testOutputHelper.WriteLine($"FeitelijkeVerenigingWerdGeregistreerd: " + JsonConvert.SerializeObject(savedEvent));

        Response.Should().NotBeNull();

        var policyResult = await Policy.Handle<Exception>()
                                       .RetryAsync(5, async (_, i) => await Task.Delay(TimeSpan.FromSeconds(i * 3)))
                                       .ExecuteAndCaptureAsync(async () =>
                                        {
                                            await using var session = _fixture.DocumentStore.LightweightSession();
                                            var stream = await session.Events.FetchStreamAsync(savedEvent.VCode);

                                            var werdenOvergenomen = stream.OfType<AdresWerdOvergenomenUitAdressenregister>();

                                            _testOutputHelper.WriteLine($"Number of events found: " + werdenOvergenomen);

                                            if (werdenOvergenomen.Count() == Request.Locaties.Length)
                                            {
                                                using (new AssertionScope())
                                                {
                                                    // Affligem locatie
                                                    var werdOvergenomenAffligem = werdenOvergenomen.ElementAt(0);
                                                    werdOvergenomenAffligem.Should().NotBeNull();
                                                    werdOvergenomenAffligem.OvergenomenAdresUitGrar.AdresId.Should().Be("2208355");
                                                    werdOvergenomenAffligem.OvergenomenAdresUitGrar.Adres.Gemeente.Should().Be("Affligem");

                                                    // Hekene locatie
                                                    var werdOvergenomenHekene = werdenOvergenomen.ElementAt(1);
                                                    werdOvergenomenHekene.Should().NotBeNull();
                                                    werdOvergenomenHekene.OvergenomenAdresUitGrar.AdresId.Should().Be("2208355");
                                                    werdOvergenomenHekene.OvergenomenAdresUitGrar.Adres.Gemeente.Should().Be("Hekene (Affligem)");

                                                    // Nothingham locatie
                                                    var werdOvergenomenNothingham = werdenOvergenomen.ElementAt(2);
                                                    werdOvergenomenNothingham.Should().NotBeNull();
                                                    werdOvergenomenNothingham.OvergenomenAdresUitGrar.AdresId.Should().Be("2208355");
                                                    werdOvergenomenNothingham.OvergenomenAdresUitGrar.Adres.Gemeente.Should().Be("Affligem");
                                                }
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