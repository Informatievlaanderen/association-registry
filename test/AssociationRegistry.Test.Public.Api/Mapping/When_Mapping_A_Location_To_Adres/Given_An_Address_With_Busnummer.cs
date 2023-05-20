namespace AssociationRegistry.Test.Public.Api.Mapping.When_Mapping_A_Location_To_Adres;

using Events;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Address_With_Busnummer
{
    [Fact]
    public void Then_Adres_contains_Busnummer()
    {
        const string straatnaam = "Den berg";
        const string huisnummer = "7";
        const string postcode = "9000";
        const string gemeente = "Gent";
        const string land = "België";
        const string busnummer = "1B";
        var locatie = new FeitelijkeVerenigingWerdGeregistreerd.Locatie(
            string.Empty, straatnaam, huisnummer, busnummer, postcode, gemeente, land, true, "Activiteiten");

        locatie.ToAdresString().Should().Be($"{straatnaam} {huisnummer} bus {busnummer}, {postcode} {gemeente}, {land}");
    }
}
