namespace AssociationRegistry.Test.Admin.Api.Grar.When_Heradresseren_Locaties;

using AssociationRegistry.Grar.Models;
using Events;
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
                                                    new Registratiedata.AdresId("AR", "https://data.vlaanderen.be/id/adres/200001"),
                                                    "Goorbaan 59, 2230 Herselt",
                                                    "Goorbaan",
                                                    "59",
                                                    string.Empty,
                                                    "2230",
                                                    "Herselt"));
    }
}

