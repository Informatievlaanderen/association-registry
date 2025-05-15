namespace AssociationRegistry.Test.Admin.Api.DuplicateDetection.Given_Gemeentenamen_In_Verenigingsnaam;

using AssociationRegistry.Admin.Api.Adapters.DuplicateVerenigingDetectionService;
using FluentAssertions;
using GemeentenaamDecorator;
using Vereniging;
using Xunit;

public class Then_Gemeentenaam_Is_Removed_From_Verenigingsnaam
{

    public Then_Gemeentenaam_Is_Removed_From_Verenigingsnaam()
    {

    }

    [Theory]
    [InlineData("Spurs","spurs", "kortrijk")]
    [InlineData("Spurs kortrijk","spurs", "KortrIjk")]
    [InlineData("KorTrijk Spurs","spurs", "kortrijk")]
    [InlineData("Halle Spurs","spurs", "Halle", "Buizingen")]
    [InlineData("Buizingen Spurs","spurs", "Halle", "Buizingen")]
    [InlineData("FC halle Spurs","fc spurs", "Halle", "Buizingen")]
    [InlineData("FC halle Spurs in buizingen","fc spurs in", "Halle", "Buizingen")]
    [InlineData("FC halLe-buiZIngen Spurs","fc halle-buizingen spurs", "Halle", "Buizingen")]
    [InlineData("HC sint-martens-latem Spurs","hc spurs", "sint-martens-latem", "Buizingen")]
    [InlineData("Halse vereniging","halse vereniging", "halle", "Buizingen")]
    [InlineData("De haan haantjes","haantjes", "de haan", "Buizingen")]
    [InlineData("De haantjes de haan","de haantjes", "de haan", "Buizingen")]
    [InlineData("De haantjes (de haan)","de haantjes ()", "de haan", "Buizingen")]
    public async ValueTask XXX(string verenigingsnaam, string expectedVerenigingsnaam, params string[] gemeentenamen)
    {

        var gemeentes = gemeentenamen.Select(x => VerrijkteGemeentenaam.FromGemeentenaam(new Gemeentenaam(x))).ToArray();
        var actual = RemoveGemeentenaamFromVerenigingsNaam.Remove(VerenigingsNaam.Create(verenigingsnaam), gemeentes);

        actual.Should().Be(expectedVerenigingsnaam);
    }
}
