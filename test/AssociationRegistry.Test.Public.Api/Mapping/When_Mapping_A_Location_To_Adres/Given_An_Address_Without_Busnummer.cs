namespace AssociationRegistry.Test.Public.Api.Mapping.When_Mapping_A_Location_To_Adres;

using Events;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Address_Without_Busnummer
{
    [Fact]
    public void Then_Adres_does_not_contain_Busnummer()
    {
        const string straatnaam = "Den berg";
        const string huisnummer = "7";
        const string postcode = "9000";
        const string gemeente = "Gent";
        const string land = "België";

        var locatie = new Registratiedata.Locatie(
            1, string.Empty, straatnaam, huisnummer, string.Empty, postcode, gemeente, land, true, "Activiteiten");

        locatie.ToAdresString().Should().Be($"{straatnaam} {huisnummer}, {postcode} {gemeente}, {land}");
    }
}
