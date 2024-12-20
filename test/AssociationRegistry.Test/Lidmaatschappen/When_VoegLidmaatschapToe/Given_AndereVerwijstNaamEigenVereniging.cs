namespace AssociationRegistry.Test.Lidmaatschappen.When_VoegLidmaatschapToe;

using AssociationRegistry.Acties.VoegLidmaatschapToe;
using AssociationRegistry.Events;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
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
