namespace AssociationRegistry.Test.When_VoegLidmaatschapToe;

using Acties.VoegLidmaatschapToe;
using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Resources;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;

public class Given_AndereVerwijstNaamEigenVereniging
{
    [Fact]
    public void Then_Throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var sut = new VerenigingOfAnyKind();
        var feitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        sut.Hydrate(new VerenigingState().Apply(feitelijkeVerenigingWerdGeregistreerd));

        var toeTeVoegenLidmaatschap = fixture.Create<VoegLidmaatschapToeCommand.ToeTeVoegenLidmaatschap>()
            with
            {
                AndereVereniging = VCode.Create(feitelijkeVerenigingWerdGeregistreerd.VCode),
            };

        var exception = Assert.Throws<LidmaatschapMagNietVerwijzenNaarEigenVereniging>(
            () => sut.VoegLidmaatschapToe(toeTeVoegenLidmaatschap));

        exception.Message.Should().Be(ExceptionMessages.LidmaatschapMagNietVerwijzenNaarEigenVereniging);
    }
}
