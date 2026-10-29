namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_CorrigeerMarkeringAlsDubbelVan.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.CorrigeerMarkeringAlsDubbelVan;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Xunit;

public class Given_An_Authentieke_Vereniging
{
    private readonly Fixture _fixture;

    public Given_An_Authentieke_Vereniging()
    {
        _fixture = new Fixture().CustomizeDomain();
    }

    [Theory]
    [InlineData(VerenigingAanvaarddeDubbeleVerenigingScenario.Verenigingstype.Feitelijke)]
    public async ValueTask Then_It_Throws(VerenigingAanvaarddeDubbeleVerenigingScenario.Verenigingstype verenigingstype)
    {
        var scenario = new VerenigingAanvaarddeDubbeleVerenigingScenario(verenigingstype);
        var aggregateSessionMock = new MultipleVerenigingAggregateSessionMock(
            scenario.GetVerenigingState(),
            expectedLoadingDubbel: true
        );

        var commandHandler = new CorrigeerMarkeringAlsDubbelVanCommandHandler(aggregateSessionMock);

        var command = _fixture.Create<CorrigeerMarkeringAlsDubbelVanCommand>() with { VCode = scenario.VCode };

        var exception = await Assert.ThrowsAsync<VerenigingMoetGemarkeerdZijnAlsDubbelOmGecorrigeerdTeKunnenWorden>(
            async () =>
                await commandHandler.Handle(
                    new CommandEnvelope<CorrigeerMarkeringAlsDubbelVanCommand>(
                        command,
                        _fixture.Create<CommandMetadata>()
                    )
                )
        );

        exception
            .Message.Should()
            .Be(ExceptionMessages.VerenigingMoetGemarkeerdZijnAlsDubbelOmGecorrigeerdTeKunnenWorden);

        aggregateSessionMock.SavedInvocations.Should().BeEmpty();
    }
}
