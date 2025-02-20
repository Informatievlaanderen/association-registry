namespace AssociationRegistry.Test.Lidmaatschappen.When_VoegLidmaatschapToe;

using AssociationRegistry.Events;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using AutoFixture.Kernel;
using DecentraalBeheer.Lidmaatschappen.VoegLidmaatschapToe;
using FluentAssertions;
using Xunit;

public class Given_AndereVerwijstNaamEigenVereniging
{
    [Theory]
    [InlineData(typeof(FeitelijkeVerenigingWerdGeregistreerd))]
    [InlineData(typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd))]
    public void Then_Throws(Type verenigingType)
    {
        var fixture = new Fixture().CustomizeDomain();
        var context = new SpecimenContext(fixture);

        var sut = new VerenigingOfAnyKind();
        var verenigingWerdGeregistreerd = (IVerenigingWerdGeregistreerd)context.Resolve(verenigingType);
        sut.Hydrate(new VerenigingState().Apply((dynamic)verenigingWerdGeregistreerd));

        var toeTeVoegenLidmaatschap = fixture.Create<VoegLidmaatschapToeCommand.ToeTeVoegenLidmaatschap>()
            with
            {
                AndereVereniging = VCode.Create(verenigingWerdGeregistreerd.VCode),
            };

        var exception = Assert.Throws<LidmaatschapMagNietVerwijzenNaarEigenVereniging>(
            () => sut.VoegLidmaatschapToe(toeTeVoegenLidmaatschap));

        exception.Message.Should().Be(ExceptionMessages.LidmaatschapMagNietVerwijzenNaarEigenVereniging);
    }
}
