namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Net;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerFeitelijkeVereniging_WithMinimalFields
{
    private static When_RegistreerFeitelijkeVereniging_WithMinimalFields? called;
    public readonly RegistreerFeitelijkeVerenigingRequest Request;
    public readonly HttpResponseMessage Response;

    private When_RegistreerFeitelijkeVereniging_WithMinimalFields(AdminApiFixture fixture)
    {
        Request = new RegistreerFeitelijkeVerenigingRequest
        {
            Naam = new Fixture().Create<string>(),
        };

        Response ??= fixture.DefaultClient.RegistreerFeitelijkeVereniging(GetJsonBody(Request)).GetAwaiter().GetResult();
    }

    public static When_RegistreerFeitelijkeVereniging_WithMinimalFields Called(AdminApiFixture fixture)
        => called ??= new When_RegistreerFeitelijkeVereniging_WithMinimalFields(fixture);

    private string GetJsonBody(RegistreerFeitelijkeVerenigingRequest request)
        => GetType()
          .GetAssociatedResourceJson("files.request.with_minimal_fields")
          .Replace(oldValue: "{{vereniging.naam}}", request.Naam);
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_Minimal_Fields
{
    private readonly EventsInDbScenariosFixture _fixture;

    public With_Minimal_Fields(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    private RegistreerFeitelijkeVerenigingRequest Request
        => When_RegistreerFeitelijkeVereniging_WithMinimalFields.Called(_fixture).Request;

    private HttpResponseMessage Response
        => When_RegistreerFeitelijkeVereniging_WithMinimalFields.Called(_fixture).Response;

    [Fact]
    public void Then_it_saves_the_events()
    {
        using var session = _fixture.DocumentStore
                                    .LightweightSession();

        session.Events.QueryRawEventDataOnly<FeitelijkeVerenigingWerdGeregistreerd>()
               .Where(e => e.Naam == Request.Naam)
               .Should().HaveCount(expected: 1);
    }

    [Fact]
    public void Then_it_returns_an_accepted_response_with_correct_headers()
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
}
