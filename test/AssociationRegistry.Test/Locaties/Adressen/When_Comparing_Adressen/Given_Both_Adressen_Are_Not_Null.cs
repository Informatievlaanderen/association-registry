namespace AssociationRegistry.Test.Locaties.Adressen.When_Comparing_Adressen;

using AssociationRegistry.Normalizers;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging.Adressen;
using Moq;
using Xunit;

public class Given_Both_Adressen_Are_Not_Null
{
    private readonly Mock<IStringNormalizer> _mock;
    private readonly Adres? _adres1;
    private readonly Adres? _adres2;

    public Given_Both_Adressen_Are_Not_Null()
    {
        var fixture = new Fixture().CustomizeDomain();
        _mock = new Mock<IStringNormalizer>();
        var sut = new AdresComparer(_mock.Object);

        _adres1 = fixture.Create<Adres>();
        _adres2 = fixture.Create<Adres>();
        sut.HasDuplicates(_adres1, _adres2);
    }

    [Fact]
    public void Then_The_StringComparer_Is_Called_For_Straatnaam()
    {
        _mock.Verify(v => v.NormalizeString(_adres1!.Straatnaam));
        _mock.Verify(v => v.NormalizeString(_adres2!.Straatnaam));
    }

    [Fact]
    public void Then_The_StringComparer_Is_Called_For_Huisnummer()
    {
        _mock.Verify(v => v.NormalizeString(_adres1!.Huisnummer));
        _mock.Verify(v => v.NormalizeString(_adres2!.Huisnummer));
    }

    [Fact]
    public void Then_The_StringComparer_Is_Called_For_Gemeente()
    {
        _mock.Verify(v => v.NormalizeString(_adres1!.Gemeente.Naam));
        _mock.Verify(v => v.NormalizeString(_adres2!.Gemeente.Naam));
    }

    [Fact]
    public void Then_The_StringComparer_Is_Called_For_Busnummer()
    {
        _mock.Verify(v => v.NormalizeString(_adres1!.Busnummer));
        _mock.Verify(v => v.NormalizeString(_adres2!.Busnummer));
    }

    [Fact]
    public void Then_The_StringComparer_Is_Called_For_Land()
    {
        _mock.Verify(v => v.NormalizeString(_adres1!.Land));
        _mock.Verify(v => v.NormalizeString(_adres2!.Land));
    }

    [Fact]
    public void Then_The_StringComparer_Is_Called_For_Postcode()
    {

        _mock.Verify(v => v.NormalizeString(_adres1!.Postcode));
        _mock.Verify(v => v.NormalizeString(_adres2!.Postcode));
    }
}
