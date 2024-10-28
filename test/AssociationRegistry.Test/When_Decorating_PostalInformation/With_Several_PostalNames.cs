namespace AssociationRegistry.Test.When_Decorating_PostalInformation;

using AutoFixture;
using Events;
using FluentAssertions;
using Grar.Models.PostalInfo;
using Xunit;
using Postnaam = Grar.Models.PostalInfo.Postnaam;

public class With_Several_PostalNames
{
    private static Fixture _fixture;

    public With_Several_PostalNames()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void And_Originele_Gemeentenaam_Is_Not_In_PostalInfo_Then_Take_Gemeente_From_PostalInfo()
    {
        var result = DecorateWithOnePostalName(
            gemeentenaam: "Affligem",
            postnamen: ["Essene", "Hekelgem", "Teralfene"],
            locatieGemeentenaam: _fixture.Create<string>());

        result.Should().BeEquivalentTo(VerrijkteGemeentenaam.ZonderPostnaam("Affligem"));
    }

    [Fact]
    public void And_PostName_Exists_Then_Verrijk_Gemeentenaam_With_Postnaam()
    {
        var locatieGemeentenaam = "HEKELGEM";

        var gemeentenaam = "Affligem";

        var result = DecorateWithOnePostalName(
            gemeentenaam: gemeentenaam,
            postnamen: ["AFFLIGEM", "Essene", "Hekelgem", "Teralfene"],
            locatieGemeentenaam: locatieGemeentenaam);

        result.Should().BeEquivalentTo(VerrijkteGemeentenaam.MetPostnaam(Postnaam.FromValue("Hekelgem"), gemeentenaam));
    }

    private static VerrijkteGemeentenaam DecorateWithOnePostalName(string gemeentenaam, string[] postnamen, string locatieGemeentenaam)
    {
        var postalInformationResponse = new PostalInformationResponse(
            Postcode: _fixture.Create<string>(),
            Gemeentenaam: gemeentenaam,
            Postnamen.FromValues(postnamen));

        var result = GemeentenaamDecorator.VerrijkGemeentenaam(origineleGemeentenaam: locatieGemeentenaam,
                                                               postalInformationResponse: postalInformationResponse,
                                                               gemeentenaamUitGrar: _fixture.Create<string>());

        return result;
    }
}
