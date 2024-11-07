namespace AssociationRegistry.Test.Public.Api.Mapping.When_Mapping_A_Location_To_Adres;

using AssociationRegistry.Public.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Public.Api.Verenigingen.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using FluentAssertions;
using Framework;
using Moq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Null_AdresId
{
    [Fact]
    public void Then_AdresId_Is_Null()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var publiekVerenigingDetailDocument = fixture.Create<PubliekVerenigingDetailDocument>();
        publiekVerenigingDetailDocument.Locaties.First().AdresId = null;
        var publiekVerenigingDetailResponse = PubliekVerenigingDetailMapper.Map(publiekVerenigingDetailDocument, new AppSettings(), Mock.Of<INamenVoorLidmaatschapMapper>());
        publiekVerenigingDetailResponse.Vereniging.Locaties.First().AdresId.Should().BeNull();
    }
}
