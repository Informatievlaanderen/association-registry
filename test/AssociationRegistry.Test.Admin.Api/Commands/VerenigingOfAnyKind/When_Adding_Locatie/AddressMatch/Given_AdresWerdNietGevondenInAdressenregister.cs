namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Locatie.AddressMatch;

using Events;
using AssociationRegistry.Framework;
using Common.Scenarios.EventsInDb;
using FluentAssertions;
using Framework.Fixtures;
using Marten;
using Polly;
using System.Net;
using Xunit;
using Xunit.Categories;

public class Given_With_AdresWerdNietGevondenInAdressenregister_Setup : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly string _jsonBody;

    public V067_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields_ForAddingLocatie_For_AdresWerdNietGevondenInAdressenregister
        Scenario { get; }

    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

    public Given_With_AdresWerdNietGevondenInAdressenregister_Setup(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

        Scenario = fixture
           .V067FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForAddingLocatieForAdresWerdNietGevondenInAdressenregister;

        DocumentStore = _fixture.DocumentStore;

        _jsonBody = @"{
            ""locatie"": {
                ""locatietype"": ""Correspondentie"",
                ""isPrimair"": true,
                ""naam"": ""nieuwe locatie"",
                ""adres"": {
                    ""straatnaam"": ""Dorpelstraat"",
                    ""huisnummer"": ""169"",
                    ""busnummer"": ""2"",
                    ""postcode"": ""4567"",
                    ""gemeente"": ""Nothingham"",
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
public class Given_AdresWerdNietGevondenInAdressenregister : IClassFixture<
    Given_With_AdresWerdNietGevondenInAdressenregister_Setup>
{
    private readonly Given_With_AdresWerdNietGevondenInAdressenregister_Setup _classFixture;

    public Given_AdresWerdNietGevondenInAdressenregister(
        Given_With_AdresWerdNietGevondenInAdressenregister_Setup classFixture)
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
                                                     Straatnaam: "Dorpelstraat",
                                                     Huisnummer: "169",
                                                     Busnummer: "2",
                                                     Postcode: "4567",
                                                     Gemeente: "Nothingham",
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
        var policyResult = await Policy.Handle<Exception>()
                                       .RetryAsync(5, async (_, i) => await Task.Delay(TimeSpan.FromSeconds(i)))
                                       .ExecuteAndCaptureAsync(async () =>
                                        {
                                            await using var session = _classFixture.DocumentStore.LightweightSession();

                                            session
                                               .SingleOrDefaultFromStream<AdresWerdNietGevondenInAdressenregister>(
                                                    _classFixture.Scenario.VCode)
                                                .Should()
                                               .NotBeNull();
                                        });

        policyResult.FinalException.Should().BeNull();
    }
}
