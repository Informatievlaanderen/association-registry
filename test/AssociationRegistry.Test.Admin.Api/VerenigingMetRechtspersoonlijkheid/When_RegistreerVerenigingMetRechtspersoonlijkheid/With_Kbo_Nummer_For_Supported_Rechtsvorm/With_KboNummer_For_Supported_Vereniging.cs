namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.
    With_Kbo_Nummer_For_Supported_Rechtsvorm;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Magda.Models;
using Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Net;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
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
    public async Task Then_it_stores_a_call_reference()
    {
        var hasCorrelationIdHeader =
            RegistreerVerenigingMetRechtspersoonlijkheidSetup.Response.Headers.TryGetValues(
                WellknownHeaderNames.CorrelationId, out var correlationIdValues);

        hasCorrelationIdHeader.Should().BeTrue();

        await using var lightweightSession = _fixture.DocumentStore.LightweightSession();

        var correlationId = Guid.Parse(correlationIdValues!.First());

        var callReference = lightweightSession
                           .Query<MagdaCallReference>()
                           .Where(x => x.CorrelationId == correlationId)
                           .SingleOrDefault();

        callReference.Should().NotBeNull();

        callReference.Should().BeEquivalentTo(
            new MagdaCallReference
            {
                CorrelationId = correlationId,
                Context = "Registreer vereniging met rechtspersoonlijkheid",
                AanroependeDienst = "Verenigingsregister Beheer Api",
                OpgevraagdeDienst = "GeefOndernemingDienst-02.00",
                OpgevraagdOnderwerp = RegistreerVerenigingMetRechtspersoonlijkheidSetup.UitKboRequest.KboNummer,
                Initiator = "OVO000001",
            },
            config: options => options.Excluding(x => x.CalledAt).Excluding(x => x.Reference));

        callReference!.Reference.Should().NotBeEmpty();
        callReference.CalledAt.Should().BeWithin(TimeSpan.FromDays(1));
    }
}
