namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.With_Kbo_Nummer_For_Supported_Rechtsvorm;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Magda.Models;
using Events;
using Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public abstract class With_KboNummer_For_Supported_Vereniging
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly RegistreerVereniginMetRechtspersoonlijkheidSetup _registreerVereniginMetRechtspersoonlijkheidSetup;

    public With_KboNummer_For_Supported_Vereniging(EventsInDbScenariosFixture fixture, RegistreerVereniginMetRechtspersoonlijkheidSetup registreerVereniginMetRechtspersoonlijkheidSetup)
    {
        _fixture = fixture;
        _registreerVereniginMetRechtspersoonlijkheidSetup = registreerVereniginMetRechtspersoonlijkheidSetup;
    }

    [Fact]
    public void Then_it_saves_the_events()
    {
        using var session = _fixture.DocumentStore
            .LightweightSession();

        session.Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
            .Where(e => e.KboNummer == _registreerVereniginMetRechtspersoonlijkheidSetup.UitKboRequest.KboNummer)
            .Should().HaveCount(expected: 1);
    }

    [Fact]
    public void Then_it_returns_an_accepted_response_with_correct_headers()
    {
        _registreerVereniginMetRechtspersoonlijkheidSetup.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public void Then_it_returns_a_location_header()
    {
        _registreerVereniginMetRechtspersoonlijkheidSetup.Response.Headers.Should().ContainKey(HeaderNames.Location);
        _registreerVereniginMetRechtspersoonlijkheidSetup.Response.Headers.Location!.OriginalString.Should()
            .StartWith($"{_fixture.ServiceProvider.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");
    }

    [Fact]
    public void Then_it_returns_a_sequence_header()
    {
        _registreerVereniginMetRechtspersoonlijkheidSetup.Response.Headers.Should().ContainKey(WellknownHeaderNames.Sequence);
        var sequenceValues = _registreerVereniginMetRechtspersoonlijkheidSetup.Response.Headers.GetValues(WellknownHeaderNames.Sequence).ToList();
        sequenceValues.Should().HaveCount(expected: 1);
        var sequence = Convert.ToInt64(sequenceValues.Single());
        sequence.Should().BeGreaterThan(expected: 0);
    }

    [Fact]
    public async Task Then_it_stores_a_call_reference()
    {
        var hasCorrelationIdHeader = _registreerVereniginMetRechtspersoonlijkheidSetup.Response.Headers.TryGetValues(WellknownHeaderNames.CorrelationId, out var correlationIdValues);

        hasCorrelationIdHeader.Should().BeTrue();

        await using var lightweightSession = _fixture.DocumentStore.LightweightSession();


        var callReferences = lightweightSession
            .Query<MagdaCallReference>()
            .ToList();

        var correlationId = Guid.Parse(correlationIdValues.First());

        var callReference = lightweightSession
            .Query<MagdaCallReference>()
            .Where(x => x.CorrelationId == correlationId)
            .SingleOrDefault();

        callReference.Should().NotBeNull();
        callReference.Should().BeEquivalentTo(
            new MagdaCallReference()
            {
                CorrelationId = correlationId,
                Context = "Registreer vereniging met rechtspersoonlijkheid",
                AanroependeDienst = "Verenigingsregister Beheer Api",
                OpgevraagdeDienst = "GeefOndernemingDienst-02.00",
                OpgevraagdOnderwerp = _registreerVereniginMetRechtspersoonlijkheidSetup.UitKboRequest.KboNummer,
                Initiator = "OVO000001",
            },
            options => options.Excluding(x => x.CalledAt).Excluding(x => x.Reference));
        callReference.Reference.Should().NotBeEmpty();
        callReference.CalledAt.Should().BeWithin(TimeSpan.FromDays(1));
    }
}
