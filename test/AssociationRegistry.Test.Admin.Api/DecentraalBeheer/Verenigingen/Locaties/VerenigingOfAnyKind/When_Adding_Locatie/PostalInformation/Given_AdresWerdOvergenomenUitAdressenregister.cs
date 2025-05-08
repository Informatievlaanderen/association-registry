namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Adding_Locatie.PostalInformation;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using Marten;
using Polly;
using System.Net;
using Xunit;
using Policy = Polly.Policy;

public class Given_AdresWerdOvergenomenUitAdressenregister_Setup : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly string _jsonBody;

    public V071_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields_ForAddingLocatie_For_PostalInformation
        Scenario { get; }

    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

    public Given_AdresWerdOvergenomenUitAdressenregister_Setup(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

        Scenario = fixture
           .V071FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForAddingLocatieForPostalInformation;

        DocumentStore = _fixture.DocumentStore;

        _jsonBody = @"{
            ""locatie"": {
                ""locatietype"": ""Correspondentie"",
                ""isPrimair"": true,
                ""naam"": ""nieuwe locatie"",
                ""adres"": {
                    ""straatnaam"": ""Fosselstraat"",
                    ""huisnummer"": ""48"",
                    ""busnummer"": """",
                    ""postcode"": ""1790"",
                    ""gemeente"": ""Affligem"",
                    ""land"": ""België"",
                }
            }
        }";
    }

    public async ValueTask InitializeAsync()
    {
        Response = await _fixture.AdminApiClient.PostLocatie(Scenario.VCode, _jsonBody);
    }

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;
}

[Collection(nameof(AdminApiCollection))]
public class Given_AdresWerdOvergenomenUitAdressenregister : IClassFixture<
    Given_AdresWerdOvergenomenUitAdressenregister_Setup>
{
    private readonly Given_AdresWerdOvergenomenUitAdressenregister_Setup _classFixture;

    public Given_AdresWerdOvergenomenUitAdressenregister(
        Given_AdresWerdOvergenomenUitAdressenregister_Setup classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async ValueTask Then_it_saves_the_events()
    {
        await using var session = _classFixture.DocumentStore.LightweightSession();

        var contactgegevenWerdToegevoegd = (await session.Events
                                                         .FetchStreamAsync(_classFixture.Scenario.VCode))
           .Single(e => e.Data.GetType() == typeof(LocatieWerdToegevoegd));

        contactgegevenWerdToegevoegd.Data.Should()
                                    .BeEquivalentTo(
                                         new LocatieWerdToegevoegd(
                                             new Registratiedata.Locatie(
                                                 LocatieId: 1,
                                                 Locatietype: "Correspondentie",
                                                 IsPrimair: true,
                                                 Naam: "nieuwe locatie",
                                                 new Registratiedata.Adres(
                                                     Straatnaam: "Fosselstraat",
                                                     Huisnummer: "48",
                                                     Busnummer: "",
                                                     Postcode: "1790",
                                                     Gemeente: "Affligem",
                                                     Land: "België"),
                                                 AdresId: null)));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async ValueTask Then_it_should_have_placed_message_on_sqs_for_address_match()
    {
        var policyResult = await Policy.Handle<Exception>()
                                       .RetryAsync(retryCount: 5, onRetryAsync: async (_, i) => await Task.Delay(TimeSpan.FromSeconds(i)))
                                       .ExecuteAndCaptureAsync(async () =>
                                        {
                                            await using var session = _classFixture.DocumentStore.LightweightSession();

                                            var werdOvergenomen =
                                                session.SingleOrDefaultFromStream<AdresWerdOvergenomenUitAdressenregister>(
                                                    _classFixture.Scenario.VCode);

                                            werdOvergenomen.Should().NotBeNull();

                                            werdOvergenomen.AdresId.Should().BeEquivalentTo(
                                                new Registratiedata.AdresId(
                                                    Broncode: "AR", Bronwaarde: "https://data.vlaanderen.be/id/adres/2208355"));
                                        });

        policyResult.FinalException.Should().BeNull();
    }
}
