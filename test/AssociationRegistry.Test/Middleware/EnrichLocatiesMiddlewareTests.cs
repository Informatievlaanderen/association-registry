namespace AssociationRegistry.Test.Middleware;

using AssociationRegistry.Framework;
using AssociationRegistry.Grar.Exceptions;
using AssociationRegistry.Grar.Models;
using AutoFixture;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Middleware;
using Common.AutoFixture;
using Common.StubsMocksFakes.Faktories;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using FluentAssertions;
using AssociationRegistry.Integrations.Grar.Clients;
using Moq;
using Xunit;

public class EnrichLocatiesMiddlewareTests
{
    private readonly Fixture _fixture;

    public EnrichLocatiesMiddlewareTests()
    {
        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Fact]
    public async Task WithNoLocaties_ReturnsEmptyEnrichedLocaties_ShouldSkipEnrichment()
    {
        var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
        {
            Locaties = [],
        };

        var actual = await EnrichLocatiesMiddleware.BeforeAsync(
            new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(command, _fixture.Create<CommandMetadata>()),
            Mock.Of<IGrarClient>());

        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task WithLocaties_ReturnsEnrichedLocaties()
    {
        var locatieWithAdres = _fixture.Create<Locatie>() with { AdresId = null, Adres = _fixture.Create<Adres>() };
        var locatieWithAdresId = _fixture.Create<Locatie>() with { AdresId = _fixture.Create<AdresId>(), Adres = null };

        var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
        {
            Locaties = [locatieWithAdresId, locatieWithAdres]
        };

        var commandEnvelope =
            new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(command, _fixture.Create<CommandMetadata>());

        var grarAdres = _fixture.Create<AddressDetailResponse>()
            with{IsActief = true};
        var grarClient = Faktory.New().GrarClientFactory.GetAdresByIdReturnsAdres(locatieWithAdresId.AdresId.ToId(), grarAdres);

        var actual = await EnrichLocatiesMiddleware.BeforeAsync(
            commandEnvelope,
            grarClient.Object);

        actual.Should().BeEquivalentTo(new Dictionary<string, Adres>()
        {
            {
                locatieWithAdresId.AdresId.Bronwaarde,
                Adres.Create(grarAdres.Straatnaam, grarAdres.Huisnummer,
                             grarAdres.Busnummer, grarAdres.Postcode,
                             grarAdres.Gemeente, Adres.België)
            }
        });
    }

    [Fact]
    public async Task WithDuplicateAdresIds_ReturnsDistinctEnrichedLocaties()
    {
        var locatieWithAdresId = _fixture.Create<Locatie>() with { AdresId = _fixture.Create<AdresId>(), Adres = null };

        var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
        {
            Locaties = [locatieWithAdresId, locatieWithAdresId]
        };

        var commandEnvelope =
            new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(command, _fixture.Create<CommandMetadata>());

        var grarAdres = _fixture.Create<AddressDetailResponse>()
            with{IsActief = true};
        var grarClient = Faktory.New().GrarClientFactory.GetAdresByIdReturnsAdres(locatieWithAdresId.AdresId.ToId(), grarAdres);

        var actual = await EnrichLocatiesMiddleware.BeforeAsync(
            commandEnvelope,
            grarClient.Object);

        actual.Should().BeEquivalentTo(new Dictionary<string, Adres>()
        {
            {
                locatieWithAdresId.AdresId.Bronwaarde,
                Adres.Create(grarAdres.Straatnaam, grarAdres.Huisnummer,
                             grarAdres.Busnummer, grarAdres.Postcode,
                             grarAdres.Gemeente, Adres.België)
            }
        });
    }

    [Fact]
    public async Task WithInactiefAdresIds_ThenThrowsException()
    {
        var locatieWithAdresId = _fixture.Create<Locatie>() with { AdresId = _fixture.Create<AdresId>(), Adres = null };

        var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
        {
            Locaties = [locatieWithAdresId, locatieWithAdresId]
        };

        var commandEnvelope =
            new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(command, _fixture.Create<CommandMetadata>());

        var grarAdres = _fixture.Create<AddressDetailResponse>() with
        {
            IsActief = false,
        };

        var grarClient = Faktory.New().GrarClientFactory.GetAdresByIdReturnsAdres(locatieWithAdresId.AdresId.ToId(), grarAdres);

        await Assert.ThrowsAnyAsync<AdressenregisterReturnedInactiefAdres>(async () => await EnrichLocatiesMiddleware.BeforeAsync(
                                                                               commandEnvelope,
                                                                               grarClient.Object));
    }
}
