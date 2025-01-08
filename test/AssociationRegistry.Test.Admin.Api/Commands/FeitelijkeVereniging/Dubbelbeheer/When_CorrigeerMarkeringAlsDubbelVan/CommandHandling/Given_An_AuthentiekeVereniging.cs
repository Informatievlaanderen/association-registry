namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.Dubbelbeheer.When_CorrigeerMarkeringAlsDubbelVan.CommandHandling;

using AssociationRegistry.Framework;
using AssociationRegistry.Messages;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using DecentraalBeheer.Dubbelbeheer.CorrigeerMarkeringAlsDubbelVan;
using FluentAssertions;
using Marten;
using Moq;
using Wolverine;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Authentieke_Vereniging
{
    private readonly Fixture _fixture;


    public Given_An_Authentieke_Vereniging()
    {
        _fixture = new Fixture().CustomizeDomain();
    }

    [Theory]
    [InlineData(VerenigingAanvaarddeDubbeleVerenigingScenario.Verenigingstype.Feitelijke)]
    public async Task Then_It_Throws(VerenigingAanvaarddeDubbeleVerenigingScenario.Verenigingstype verenigingstype)
    {
        var scenario = new VerenigingAanvaarddeDubbeleVerenigingScenario(verenigingstype);
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var martenOutbox = new Mock<IMartenOutbox>();

        var commandHandler = new CorrigeerMarkeringAlsDubbelVanCommandHandler(
            verenigingRepositoryMock,
            martenOutbox.Object,
            Mock.Of<IDocumentSession>()
        );

        var command = _fixture.Create<CorrigeerMarkeringAlsDubbelVanCommand>() with
        {
            VCode = scenario.VCode,
        };

       var exception = await Assert.ThrowsAsync<VerenigingMoetGemarkeerdZijnAlsDubbelOmGecorrigeerdTeKunnenWorden>(async () => await commandHandler.Handle(new CommandEnvelope<CorrigeerMarkeringAlsDubbelVanCommand>(command, _fixture.Create<CommandMetadata>())));

       exception.Message.Should().Be(ExceptionMessages.VerenigingMoetGemarkeerdZijnAlsDubbelOmGecorrigeerdTeKunnenWorden);

       martenOutbox.Verify(x => x.SendAsync(It.IsAny<AanvaardDubbeleVerenigingMessage>(), It.IsAny<DeliveryOptions>()), Times.Never);
    }
}
