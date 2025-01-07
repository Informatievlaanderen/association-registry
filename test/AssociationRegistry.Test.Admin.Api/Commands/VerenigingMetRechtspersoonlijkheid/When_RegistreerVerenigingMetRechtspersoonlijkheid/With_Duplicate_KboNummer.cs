namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
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

public sealed class When_RegistreerVerenigingMetRechtspersoonlijkheid_WithDuplicateKboNummer
{
    private static When_RegistreerVerenigingMetRechtspersoonlijkheid_WithDuplicateKboNummer? called;
    public readonly RegistreerVerenigingUitKboRequest UitKboRequest;
    public readonly HttpResponseMessage Response;

    private When_RegistreerVerenigingMetRechtspersoonlijkheid_WithDuplicateKboNummer(EventsInDbScenariosFixture fixture)
    {
        UitKboRequest = new RegistreerVerenigingUitKboRequest
        {
            KboNummer = fixture.V020VerenigingMetRechtspersoonlijkheidWerdGeregistreerdForDuplicateDetection
                               .VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer,
        };

        Response ??= fixture.DefaultClient.RegistreerKboVereniging(GetJsonBody(UitKboRequest)).GetAwaiter().GetResult();
    }

    public static When_RegistreerVerenigingMetRechtspersoonlijkheid_WithDuplicateKboNummer Called(EventsInDbScenariosFixture fixture)
        => called ??= new When_RegistreerVerenigingMetRechtspersoonlijkheid_WithDuplicateKboNummer(fixture);

    private string GetJsonBody(RegistreerVerenigingUitKboRequest uitKboRequest)
        => GetType()
          .GetAssociatedResourceJson("files.request.with_kboNummer")
          .Replace(oldValue: "{{kboNummer}}", uitKboRequest.KboNummer);
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_Duplicate_KboNummer
{
    private readonly EventsInDbScenariosFixture _fixture;

    public With_Duplicate_KboNummer(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    private RegistreerVerenigingUitKboRequest Request
        => When_RegistreerVerenigingMetRechtspersoonlijkheid_WithDuplicateKboNummer.Called(_fixture).UitKboRequest;

    private HttpResponseMessage Response
        => When_RegistreerVerenigingMetRechtspersoonlijkheid_WithDuplicateKboNummer.Called(_fixture).Response;

    [Fact]
    public void Then_it_saves_no_events()
    {
        using var session = _fixture.DocumentStore
                                    .LightweightSession();

        //only the original event with kbonummer

        session.Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
               .Where(e => e.KboNummer == Request.KboNummer)
               .Should().HaveCount(expected: 1);
    }

    [Fact]
    public void Then_it_returns_an_ok_response_with_correct_headers()
    {
        Response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void Then_it_returns_a_location_header()
    {
        Response.Headers.Should().ContainKey(HeaderNames.Location);

        Response.Headers.Location!.OriginalString.Should()
                .StartWith(
                     $"{_fixture.ServiceProvider.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/{_fixture.V020VerenigingMetRechtspersoonlijkheidWerdGeregistreerdForDuplicateDetection.VCode}");
    }
}
