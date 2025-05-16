namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Wijzig_Locatie;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using Marten;
using Polly;
using System.Net;
using Xunit;
using Xunit.Categories;

public class Given_A_FeitelijkeVereniging_With_AdresNietUniekInAdressenregister_Setup : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly string _jsonBody;
    public V068_FeitelijkeVerenigingWerdGeregistreerd_WithLocaties_ForWijzigen_For_AdresNietUniekInAdressenregister Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

    public Given_A_FeitelijkeVereniging_With_AdresNietUniekInAdressenregister_Setup(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

        Scenario = fixture.V068FeitelijkeVerenigingWerdGeregistreerdWithLocatiesForWijzigenForAdresNietUniekInAdressenregister;
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
                    ""postcode"": ""1234"",
                    ""gemeente"": ""Dendermonde"",
                    ""land"": ""België"",
                }
            }
        }";
    }

    public async Task InitializeAsync()
    {
        Response = await _fixture.AdminApiClient.PatchLocatie(Scenario.VCode,
                                                              Scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties.First().LocatieId,
                                                              _jsonBody);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class Given_A_FeitelijkeVereniging_With_AdresNietUniekInAdressenregister : IClassFixture<
    Given_A_FeitelijkeVereniging_With_AdresNietUniekInAdressenregister_Setup>
{
    private readonly Given_A_FeitelijkeVereniging_With_AdresNietUniekInAdressenregister_Setup _classFixture;

    public Given_A_FeitelijkeVereniging_With_AdresNietUniekInAdressenregister(
        Given_A_FeitelijkeVereniging_With_AdresNietUniekInAdressenregister_Setup classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        await using var session = _classFixture.DocumentStore.LightweightSession();

        var locatieWerdGewijzigd = session.SingleOrDefaultFromStream<LocatieWerdGewijzigd>(_classFixture.Scenario.VCode);

        locatieWerdGewijzigd.Should()
                            .BeEquivalentTo(
                                 new LocatieWerdGewijzigd(
                                     new Registratiedata.Locatie(
                                         _classFixture.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties.First().LocatieId,
                                         Locatietype: "Correspondentie",
                                         IsPrimair: true,
                                         Naam: "nieuwe locatie",
                                         new Registratiedata.Adres(
                                             Straatnaam: "Leopold II-laan",
                                             Huisnummer: "99",
                                             Busnummer: "",
                                             Postcode: "1234",
                                             Gemeente: "Dendermonde",
                                             Land: "België"),
                                         AdresId: null)));
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
                                     .RetryAsync(retryCount: 5, onRetryAsync: async (exception, i) =>
                                      {
                                          await Task.Delay(TimeSpan.FromSeconds(i));
                                      });

        var policyResult = await asyncRetryPolicy.ExecuteAndCaptureAsync(() =>
        {
            using var session = _classFixture.DocumentStore
                                             .LightweightSession();

            session
               .SingleOrDefaultFromStream<AdresNietUniekInAdressenregister>(_classFixture.Scenario.VCode)
               .Should()
               .NotBeNull();

            return Task.CompletedTask;
        });

        policyResult.FinalException.Should().BeNull();
    }
}
