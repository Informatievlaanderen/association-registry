namespace AssociationRegistry.Test.When_Heradresseren_Locaties;

using AssociationRegistry.Events;
using AssociationRegistry.Grar.Models;
using Fixtures;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Id :  IClassFixture<DetailFixture>
{
    private readonly DetailFixture _fixture;

    public With_Id(DetailFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Then_It_Returns_A_SuccessResult()
    {
        _fixture.Result.Should().NotBeNull();

        _fixture.Result.Should().BeEquivalentTo(new AddressDetailResponse(
                                                    AdresId: new Registratiedata.AdresId("AR", "https://data.vlaanderen.be/id/adres/200001"),
                                                    IsActief: true,
                                                    Adresvoorstelling: "Goorbaan 59, 2230 Herselt",
                                                    Straatnaam: "Goorbaan",
                                                    Huisnummer: "59",
                                                    Busnummer: string.Empty,
                                                    Postcode: "2230",
                                                    Gemeente: "Herselt"));
    }
}

