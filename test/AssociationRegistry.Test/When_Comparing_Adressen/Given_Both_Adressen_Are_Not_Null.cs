namespace AssociationRegistry.Test.When_Comparing_Adressen;

using AutoFixture;
using Common.AutoFixture;
using Moq;
using Normalizers;
using Vereniging;
using Xunit;

public class Given_Both_Adressen_Are_Not_Null
{
    [Fact]
    public void Then_The_StringComparer_Is_Called_For_Every_Component()
    {
        var fixture = new Fixture().CustomizeDomain();
        var mock = new Mock<IStringNormalizer>();
        var sut = new AdresComparer(mock.Object);

        var adres1 = fixture.Create<Adres>();
        var adres2 = fixture.Create<Adres>();
        var result = sut.HasDuplicates(adres1, adres2);

        mock.Verify(v => v.NormalizeString(adres1.Straatnaam));
        mock.Verify(v => v.NormalizeString(adres1.Huisnummer));
        mock.Verify(v => v.NormalizeString(adres1.Busnummer));
        mock.Verify(v => v.NormalizeString(adres1.Gemeente));
        mock.Verify(v => v.NormalizeString(adres1.Land));
        mock.Verify(v => v.NormalizeString(adres1.Postcode));

        mock.Verify(v => v.NormalizeString(adres2.Straatnaam));
        mock.Verify(v => v.NormalizeString(adres2.Huisnummer));
        mock.Verify(v => v.NormalizeString(adres2.Busnummer));
        mock.Verify(v => v.NormalizeString(adres2.Gemeente));
        mock.Verify(v => v.NormalizeString(adres2.Land));
        mock.Verify(v => v.NormalizeString(adres2.Postcode));
    }
}
