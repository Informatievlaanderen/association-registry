namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.MetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.
    With_Kbo_Nummer_For_Supported_Rechtsvorm;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Events;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Integrations.Magda.Models;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Net;
using Xunit;

[Collection(nameof(AdminApiCollection))]
public abstract class With_KboNummer_For_Supported_Vereniging
{
    protected readonly EventsInDbScenariosFixture _fixture;
    protected readonly RegistreerVereniginMetRechtspersoonlijkheidSetup RegistreerVerenigingMetRechtspersoonlijkheidSetup;

    public With_KboNummer_For_Supported_Vereniging(
        EventsInDbScenariosFixture fixture,
        RegistreerVereniginMetRechtspersoonlijkheidSetup registreerVerenigingMetRechtspersoonlijkheidSetup)
    {
        _fixture = fixture;
        RegistreerVerenigingMetRechtspersoonlijkheidSetup = registreerVerenigingMetRechtspersoonlijkheidSetup;
    }

    [Fact]
    public void Then_it_returns_an_accepted_response_with_correct_headers()
    {
        RegistreerVerenigingMetRechtspersoonlijkheidSetup.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public void Then_it_returns_a_location_header()
    {
        RegistreerVerenigingMetRechtspersoonlijkheidSetup.Response.Headers.Should().ContainKey(HeaderNames.Location);

        RegistreerVerenigingMetRechtspersoonlijkheidSetup.Response.Headers.Location!.OriginalString.Should()
                                                         .StartWith(
                                                              $"{_fixture.ServiceProvider.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");
    }

    [Fact]
    public void Then_it_returns_a_sequence_header()
    {
        RegistreerVerenigingMetRechtspersoonlijkheidSetup.Response.Headers.Should().ContainKey(WellknownHeaderNames.Sequence);

        var sequenceValues = RegistreerVerenigingMetRechtspersoonlijkheidSetup.Response.Headers.GetValues(WellknownHeaderNames.Sequence)
                                                                              .ToList();

        sequenceValues.Should().HaveCount(expected: 1);
        var sequence = Convert.ToInt64(sequenceValues.Single());
        sequence.Should().BeGreaterThan(expected: 0);
    }

    [Fact]
    public async ValueTask Then_it_stores_a_call_reference()
    {
        var hasCorrelationIdHeader =
            RegistreerVerenigingMetRechtspersoonlijkheidSetup.Response.Headers.TryGetValues(
                WellknownHeaderNames.CorrelationId, out var correlationIdValues);

        hasCorrelationIdHeader.Should().BeTrue();

        await using var lightweightSession = _fixture.DocumentStore.LightweightSession();

        var correlationId = Guid.Parse(correlationIdValues!.First());

        var callReferences = await lightweightSession
                                  .Query<MagdaCallReference>()
                                  .Where(x => x.CorrelationId == correlationId)
                                  .ToListAsync();

        callReferences.Should().NotBeNull();

        callReferences.Should().BeEquivalentTo(
            new List<MagdaCallReference>
            {
                new()
                {
                    CorrelationId = correlationId,
                    Context = "Registreer inschrijving voor vereniging met rechtspersoonlijkheid",
                    AanroependeDienst = "Verenigingsregister Beheer Api",
                    OpgevraagdeDienst = "RegistreerInschrijvingDienst-02.01",
                    OpgevraagdOnderwerp = RegistreerVerenigingMetRechtspersoonlijkheidSetup.UitKboRequest.KboNummer,
                    Initiator = "OVO000001",
                },
                new()
                {
                    CorrelationId = correlationId,
                    Context = "Registreer vereniging met rechtspersoonlijkheid",
                    AanroependeDienst = "Verenigingsregister Beheer Api",
                    OpgevraagdeDienst = "GeefOndernemingDienst-02.00",
                    OpgevraagdOnderwerp = RegistreerVerenigingMetRechtspersoonlijkheidSetup.UitKboRequest.KboNummer,
                    Initiator = "OVO000001",
                },
            },
            config: options => options.Excluding(x => x.CalledAt).Excluding(x => x.Reference));

        foreach (var magdaCallReference in callReferences)
        {
            magdaCallReference!.Reference.Should().NotBeEmpty();
            magdaCallReference.CalledAt.Should().BeWithin(TimeSpan.FromDays(1));
        }
    }

    [Fact]
    public void Then_it_saves_the_vereniging_werd_ingeschreven_op_wijzigingen_uit_kbo_event()
    {
        using var session = _fixture
                           .DocumentStore
                           .LightweightSession();

        session
           .Events
           .QueryRawEventDataOnly<VerenigingWerdIngeschrevenOpWijzigingenUitKbo>()
           .Should().ContainSingle(
                e => e.KboNummer == RegistreerVerenigingMetRechtspersoonlijkheidSetup
                                   .UitKboRequest.KboNummer);
    }
}
