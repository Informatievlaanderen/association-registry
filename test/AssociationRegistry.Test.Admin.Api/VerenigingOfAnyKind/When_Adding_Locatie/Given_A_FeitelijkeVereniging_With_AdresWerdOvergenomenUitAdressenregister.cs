namespace AssociationRegistry.Test.Admin.Api.VerenigingOfAnyKind.When_Adding_Locatie;

using Events;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Marten;
using Polly;
using System.Net;
using Xunit;
using Xunit.Categories;

public class Given_A_FeitelijkeVereniging_With_AdresWerdOvergenomenUitAdressenregister_Setup : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly string _jsonBody;
    public V066_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields_ForAddingLocatie_For_AdresWerdOvergenomenUitAdressenregister Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

    public Given_A_FeitelijkeVereniging_With_AdresWerdOvergenomenUitAdressenregister_Setup(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

        Scenario = fixture
           .V066FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForAddingLocatieForAdresWerdOvergenomenUitAdressenregister;
        DocumentStore = _fixture.DocumentStore;

        _jsonBody = @"{
            ""locatie"": {
                ""locatietype"": ""Correspondentie"",
                ""isPrimair"": true,
                ""naam"": ""nieuwe locatie"",
                ""adres"": {
                    ""straatnaam"": ""Leopold II-laan"",
                    ""huisnummer"": ""99"",
                    ""busnummer"": """",
                    ""postcode"": ""9200"",
                    ""gemeente"": ""Dendermonde"",
                    ""land"": ""België"",
                }
            }
        }";
    }

    public async Task InitializeAsync()
    {
        Response = await _fixture.AdminApiClient.PostLocatie(Scenario.VCode, _jsonBody);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class Given_A_FeitelijkeVereniging_With_AdresWerdOvergenomenUitAdressenregister : IClassFixture<
    Given_A_FeitelijkeVereniging_With_AdresWerdOvergenomenUitAdressenregister_Setup>
{
    private readonly Given_A_FeitelijkeVereniging_With_AdresWerdOvergenomenUitAdressenregister_Setup _classFixture;

    public Given_A_FeitelijkeVereniging_With_AdresWerdOvergenomenUitAdressenregister(
        Given_A_FeitelijkeVereniging_With_AdresWerdOvergenomenUitAdressenregister_Setup classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async Task Then_it_saves_the_events()
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
                                                     Straatnaam: "Leopold II-laan",
                                                     Huisnummer: "99",
                                                     Busnummer: "",
                                                     Postcode: "9200",
                                                     Gemeente: "Dendermonde",
                                                     Land: "België"),
                                                 null)));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Then_it_should_have_placed_message_on_sqs_for_address_match()
    {
        var asyncRetryPolicy = Policy.Handle<Exception>()
                                     .RetryAsync(5, async (exception, i) =>
                                      {
                                          await Task.Delay(TimeSpan.FromSeconds(i));
                                      });

        var policyResult = await asyncRetryPolicy.ExecuteAndCaptureAsync(() =>
        {
            using var session = _classFixture.DocumentStore
                                             .LightweightSession();

            var werdOvergenomen = session.Events
                                         .QueryRawEventDataOnly<AdresWerdOvergenomenUitAdressenregister>()
                                         .SingleOrDefault();

            werdOvergenomen.Should().NotBeNull();
            werdOvergenomen.OvergenomenAdresUitGrar.AdresId.Should().Be("3213019");

            return Task.CompletedTask;
        });

        policyResult.FinalException.Should().BeNull();
    }
}
