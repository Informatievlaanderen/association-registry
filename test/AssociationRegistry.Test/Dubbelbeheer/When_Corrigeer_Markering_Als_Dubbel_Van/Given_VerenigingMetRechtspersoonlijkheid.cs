﻿namespace AssociationRegistry.Test.Dubbelbeheer.When_Corrigeer_Markering_Als_Dubbel_Van;

using AssociationRegistry.Framework;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Dubbelbeheer.CorrigeerMarkeringAlsDubbelVan;
using FluentAssertions;
using Marten;
using Moq;
using Wolverine.Marten;
using Xunit;

public class Given_VerenigingMetRechtspersoonlijkheid
{
    [Fact]
    public async ValueTask Then_Throws_VerenigingKanGeenDubbelWordenVanVerwijderdeVereniging()
    {
        var fixture = new Fixture().CustomizeDomain();
        var scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_AllFields_Scenario();
        var verenigingsRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());
        var command = fixture.Create<CorrigeerMarkeringAlsDubbelVanCommand>() with
        {
            VCode = VCode.Create(scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode),
        };
        var commandEnvelope = new CommandEnvelope<CorrigeerMarkeringAlsDubbelVanCommand>(command, fixture.Create<CommandMetadata>());

        var sut = new CorrigeerMarkeringAlsDubbelVanCommandHandler(verenigingsRepositoryMock,
                                                        Mock.Of<IMartenOutbox>(),
                                                        Mock.Of<IDocumentSession>()
        );

        var exception = await Assert.ThrowsAsync<ActieIsNietToegestaanVoorVerenigingstype>
            (async () => await sut.Handle(commandEnvelope, CancellationToken.None));

        exception.Message.Should().Be(ExceptionMessages.UnsupportedOperationForVerenigingstype);
    }
}
