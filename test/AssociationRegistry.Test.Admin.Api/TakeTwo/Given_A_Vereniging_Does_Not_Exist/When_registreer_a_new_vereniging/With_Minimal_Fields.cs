namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Given_A_Vereniging_Does_Not_Exist.When_registreer_a_new_vereniging;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Events;
using Framework;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_Minimal_Fields
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly RegistreerVerenigingRequest _request;

    public With_Minimal_Fields(EventsInDbScenariosFixture fixture)
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
