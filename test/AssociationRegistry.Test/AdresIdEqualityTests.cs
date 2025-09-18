namespace AssociationRegistry.Test;

using DecentraalBeheer.Vereniging.Adressen;
using Events;
using FluentAssertions;
using Xunit;

public class AdresIdEqualityTests
{
    private readonly AdresId EqualityComparisonSource =
        AdresId.Create(new Adresbron(code: "TEST", beschrijving: "Test beschrijving"), bronwaarde: "testwaarde");

    [Theory]
    [InlineData("TEST", "testwaarde", true)]
    [InlineData("TEST", "bronwaarde", false)]
    [InlineData("BRON", "testwaarde", false)]
    public void Compares_Broncode_And_Bronwaarde(string broncode, string bronwaarde, bool expectedResult)
    {
        var adresId = new Registratiedata.AdresId(broncode, bronwaarde);
        (EqualityComparisonSource == adresId).Should().Be(expectedResult);
        (EqualityComparisonSource.Equals(adresId)).Should().Be(expectedResult);
        (adresId == EqualityComparisonSource).Should().Be(expectedResult);
        (adresId.Equals(EqualityComparisonSource)).Should().Be(expectedResult);
    }
}
