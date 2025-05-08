namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Verwijder.FeitelijkeVereniging.When_Removing_A_Vereniging;

using AssociationRegistry.Events;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using Marten;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using Xunit;

public class Remove_A_Removed_Vereniging : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    public V059_FeitelijkeVerenigingWerdGeregistreerd_AndRemoved Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

    public Remove_A_Removed_Vereniging(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

        Scenario = fixture.V059FeitelijkeVerenigingWerdGeregistreerdAndRemoved;
        DocumentStore = _fixture.DocumentStore;
    }

    public async ValueTask InitializeAsync()
    {
        Response = await _fixture.SuperAdminApiClient.DeleteVereniging(Scenario.VCode, reason: "Omdat");
    }

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;
}

[Collection(nameof(AdminApiCollection))]
public class Given_A_Removed_Vereniging : IClassFixture<Remove_A_Removed_Vereniging>
{
    private readonly Remove_A_Removed_Vereniging _classFixture;

    public Given_A_Removed_Vereniging(Remove_A_Removed_Vereniging classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async ValueTask Then_it_saves_no_events()
    {
        await using var session = _classFixture.DocumentStore.LightweightSession();

        var verenigingWerdVerwijderd = (await session.Events
                                                     .FetchStreamAsync(_classFixture.Scenario.VCode))
           .Where(e => e.Data.GetType() == typeof(VerenigingWerdVerwijderd));

        verenigingWerdVerwijderd.Should().HaveCount(1);
    }

    [Fact]
    public void Then_it_returns_a_bad_request_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async ValueTask Then_it_returns_a_correct_message()
    {
        var responseBody = await _classFixture.Response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<JObject>(responseBody);

        problemDetails["detail"].Value<string>().Should().Be(ExceptionMessages.VerenigingIsVerwijderd);
    }
}
