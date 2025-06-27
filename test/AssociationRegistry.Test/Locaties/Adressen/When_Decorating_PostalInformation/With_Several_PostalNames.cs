namespace AssociationRegistry.Test.Locaties.Adressen.When_Decorating_PostalInformation;

using AssociationRegistry.Grar.Models.PostalInfo;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using GemeentenaamVerrijking;
using Xunit;

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
            locatieGemeentenaam: _fixture.Create<Gemeentenaam>());

        result.Should().BeEquivalentTo(VerrijkteGemeentenaam.ZonderPostnaam("Affligem"));
    }

    [Fact]
    public void And_PostName_Exists_Then_Verrijk_Gemeentenaam_With_Postnaam()
    {
        var locatieGemeentenaam = Gemeentenaam.Hydrate("HEKELGEM");

        var gemeentenaam = "Affligem";

        var result = DecorateWithOnePostalName(
            gemeentenaam: gemeentenaam,
            postnamen: ["AFFLIGEM", "Essene", "Hekelgem", "Teralfene"],
            locatieGemeentenaam: locatieGemeentenaam);

        result.Should().BeEquivalentTo(VerrijkteGemeentenaam.MetPostnaam(Postnaam.FromValue("Hekelgem"), gemeentenaam));
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
