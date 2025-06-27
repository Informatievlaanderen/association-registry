namespace AssociationRegistry.Test.Locaties.Adressen.When_Decorating_PostalInformation;

using AssociationRegistry.Grar.Models.PostalInfo;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using GemeentenaamVerrijking;
using Xunit;

public class With_No_PostalName
{
    private static Fixture _fixture;

    public With_No_PostalName()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Then_Takes_The_MunicipalityName()
    {
        var gemeentenaamUitGrar = "Affligem";

        var result = DecorateWithOnePostalName(
            gemeentenaam: gemeentenaamUitGrar,
            postnamen: [],
            locatieGemeentenaam: _fixture.Create<Gemeentenaam>());

        result.Should().BeEquivalentTo(VerrijkteGemeentenaam.ZonderPostnaam(gemeentenaamUitGrar));
    }

    private static VerrijkteGemeentenaam DecorateWithOnePostalName(string gemeentenaam, string[] postnamen, Gemeentenaam locatieGemeentenaam)
    {
        var postalInformationResponse = new PostalInfoDetailResponse(
            Postcode: _fixture.Create<string>(),
            Gemeentenaam: gemeentenaam,
            Postnamen.FromValues(postnamen));

        var result = GemeentenaamDecorator.VerrijkGemeentenaam(gemeentenaam: locatieGemeentenaam,
                                                               postalInformationResponse: postalInformationResponse,
                                                               gemeentenaamUitGrar: _fixture.Create<string>());

        return result;
    }
}
