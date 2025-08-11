namespace AssociationRegistry.Test.Lidmaatschappen.When_VoegLidmaatschapToe;

using AssociationRegistry.Events;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using AutoFixture.Kernel;
using CommandHandling.DecentraalBeheer.Acties.Lidmaatschappen.VoegLidmaatschapToe;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
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
        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);
        sut.Hydrate(new VerenigingState().Apply((dynamic)verenigingWerdGeregistreerd));

        var toeTeVoegenLidmaatschap = fixture.Create<ToeTeVoegenLidmaatschap>()
            with
            {
                AndereVereniging = VCode.Create(verenigingWerdGeregistreerd.VCode),
            };

        var exception = Assert.Throws<LidmaatschapMagNietVerwijzenNaarEigenVereniging>(
            () => sut.VoegLidmaatschapToe(toeTeVoegenLidmaatschap));

        exception.Message.Should().Be(ExceptionMessages.LidmaatschapMagNietVerwijzenNaarEigenVereniging);
    }
}
