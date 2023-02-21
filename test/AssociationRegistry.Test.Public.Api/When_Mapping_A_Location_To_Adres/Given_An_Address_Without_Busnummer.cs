namespace AssociationRegistry.Test.Public.Api.When_Mapping_A_Location_To_Adres;

using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using Events;
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

        var locatie = new VerenigingWerdGeregistreerd.Locatie(
            null, straatnaam, huisnummer, null, postcode, gemeente, land, true, "Activiteiten");

        locatie.ToAdresString().Should().Be($"{straatnaam} {huisnummer}, {postcode} {gemeente}, {land}");
    }
}
