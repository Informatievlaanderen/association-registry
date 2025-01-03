﻿namespace AssociationRegistry.Test.Dubbels.When_Corrigeer_Markering_Als_Dubbel_Van;

using Acties.CorrigeerMarkeringAlsDubbelVan;
using AssociationRegistry.Framework;
using Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using Common.Scenarios.CommandHandling;
using Vereniging;
using Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Marten;
using Moq;
using Wolverine.Marten;
using Xunit;

public class Given_GeenDubbeleVereniging
{
    [Fact]
    public async Task Then_Throws_VerenigingKanGeenDubbelWordenVanVerwijderdeVereniging()
    {
        var fixture = new Fixture().CustomizeDomain();
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var verenigingsRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());
        var command = fixture.Create<CorrigeerMarkeringAlsDubbelVanCommand>() with
        {
            VCode = VCode.Create(scenario.VCode),
        };
        var commandEnvelope = new CommandEnvelope<CorrigeerMarkeringAlsDubbelVanCommand>(command, fixture.Create<CommandMetadata>());

        var sut = new CorrigeerMarkeringAlsDubbelVanCommandHandler(verenigingsRepositoryMock,
                                                        Mock.Of<IMartenOutbox>(),
                                                        Mock.Of<IDocumentSession>()
        );

        var exception = await Assert.ThrowsAsync<VerenigingMoetGemarkeerdZijnAlsDubbelOmGecorrigeerdTeKunnenWorden>
            (async () => await sut.Handle(commandEnvelope, CancellationToken.None));

        exception.Message.Should().Be(ExceptionMessages.VerenigingMoetGemarkeerdZijnAlsDubbelOmGecorrigeerdTeKunnenWorden);
    }
}