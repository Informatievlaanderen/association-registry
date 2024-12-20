namespace AssociationRegistry.Test.GrarClient;

using Events;
using FluentAssertions;
using Grar.Models;
using Locaties.When_Heradresseren_Locaties.Fixtures;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Id : IClassFixture<DetailFixture>
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
                                                    new Registratiedata.AdresId(Broncode: "AR",
                                                                                Bronwaarde: "https://data.vlaanderen.be/id/adres/200001"),
                                                    IsActief: true,
                                                    Adresvoorstelling: "Goorbaan 59, 2230 Herselt",
                                                    Straatnaam: "Goorbaan",
                                                    Huisnummer: "59",
                                                    string.Empty,
                                                    Postcode: "2230",
                                                    Gemeente: "Herselt"));
    }
}
