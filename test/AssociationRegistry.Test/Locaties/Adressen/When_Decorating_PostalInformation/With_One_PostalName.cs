namespace AssociationRegistry.Test.Locaties.Adressen.When_Decorating_PostalInformation;

using AssociationRegistry.Grar.Models.PostalInfo;
using AssociationRegistry.Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging.Adressen;
using FluentAssertions;
using GemeentenaamVerrijking;
using Xunit;

public class With_One_PostalName
{
    private static readonly Fixture _fixture;

    static With_One_PostalName()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void And_PostalName_Differs_From_MunicipalityName_Then_Gemeentenaam_Includes_Postnaam()
    {
        var result = DecorateWithOnePostalName(
            gemeentenaam: "Ternat",
            postnaam: "Wambeek");

        result.Should().BeEquivalentTo(VerrijkteGemeentenaam.MetPostnaam(Postnaam.FromValue("Wambeek"), "Ternat"));
    }

    [Fact]
    public void And_PostalName_Is_Equivalent_To_MunicipalityName_Then_Gemeentenaam_Is_Returned()
    {
        var result = DecorateWithOnePostalName(
            gemeentenaam: "Ternat",
            postnaam: "TERNAT");

        result.Should().BeEquivalentTo(VerrijkteGemeentenaam.ZonderPostnaam("Ternat"));
    }

    private static VerrijkteGemeentenaam DecorateWithOnePostalName(string gemeentenaam, string postnaam)
    {
        var origineleGemeentenaam = _fixture.Create<Gemeentenaam>();

        var postalInformationResponse = new PostalInfoDetailResponse(
            Postcode: _fixture.Create<string>(),
            Gemeentenaam: gemeentenaam,
            Postnamen.FromValues(postnaam));

        var result = GemeentenaamDecorator.VerrijkGemeentenaam(gemeentenaam: origineleGemeentenaam,
                                                               postalInformationResponse: postalInformationResponse,
                                                               gemeentenaamUitGrar: origineleGemeentenaam.Naam);

        return result;
    }
}
