namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Given_A_Vereniging_Does_Not_Exist.When_registreer_a_new_vereniging;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Xunit;

[Collection(nameof(AdminApiCollection))]
public class Given_A_Valid_Request_With_Minimal_Fields
{
    private readonly GivenEventsFixture _fixture;
    private readonly RegistreerVerenigingRequest _request;

    public Given_A_Valid_Request_With_Minimal_Fields(GivenEventsFixture fixture)
    {
        _fixture = fixture;
        _request = new RegistreerVerenigingRequest
        {
            Naam = new Fixture().Create<string>(),
            Initiator = "OVO000001",
        };
        _fixture.AdminApiClient.RegistreerVereniging(GetJsonBody(_request)).GetAwaiter().GetResult();
    }

    private string GetJsonBody(RegistreerVerenigingRequest request)
        => GetType()
            .GetAssociatedResourceJson($"files.request.with_minimal_fields")
            .Replace("{{vereniging.naam}}", request.Naam)
            .Replace("{{vereniging.initiator}}", request.Initiator);

    [Fact]
    public void Then_it_saves_the_events()
    {
        using var session = _fixture.DocumentStore
            .LightweightSession();
        session.Events.QueryRawEventDataOnly<VerenigingWerdGeregistreerd>()
            .Where(e => e.Naam == _request.Naam)
            .Should().HaveCount(1);
    }
}
