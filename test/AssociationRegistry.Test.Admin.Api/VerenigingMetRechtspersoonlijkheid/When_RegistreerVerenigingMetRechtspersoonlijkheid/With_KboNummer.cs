namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid;
using Events;
using Fixtures;
using Framework;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerVerenigingMetRechtspersoonlijkheid_WithKboNummer
{
    private static When_RegistreerVerenigingMetRechtspersoonlijkheid_WithKboNummer? called;
    public readonly RegistreerVerenigingUitKboRequest UitKboRequest;
    public readonly HttpResponseMessage Response;

    private When_RegistreerVerenigingMetRechtspersoonlijkheid_WithKboNummer(AdminApiFixture fixture)
    {
        UitKboRequest = new RegistreerVerenigingUitKboRequest
        {
            KboNummer = new Fixture().CustomizeAdminApi().Create<KboNummer>(),
        };
        Response ??= fixture.DefaultClient.RegistreerKboVereniging(GetJsonBody(UitKboRequest)).GetAwaiter().GetResult();
    }

    public static When_RegistreerVerenigingMetRechtspersoonlijkheid_WithKboNummer Called(AdminApiFixture fixture)
        => called ??= new When_RegistreerVerenigingMetRechtspersoonlijkheid_WithKboNummer(fixture);

    private string GetJsonBody(RegistreerVerenigingUitKboRequest uitKboRequest)
        => GetType()
            .GetAssociatedResourceJson("files.request.with_kboNummer")
            .Replace("{{kboNummer}}", uitKboRequest.KboNummer);
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_KboNummer
{
    private readonly EventsInDbScenariosFixture _fixture;

    public With_KboNummer(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    private RegistreerVerenigingUitKboRequest Request
        => When_RegistreerVerenigingMetRechtspersoonlijkheid_WithKboNummer.Called(_fixture).UitKboRequest;

    private HttpResponseMessage Response
        => When_RegistreerVerenigingMetRechtspersoonlijkheid_WithKboNummer.Called(_fixture).Response;

    [Fact]
    public void Then_it_saves_the_events()
    {
        using var session = _fixture.DocumentStore
            .LightweightSession();

        session.Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
            .Where(e => e.KboNummer == Request.KboNummer)
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
