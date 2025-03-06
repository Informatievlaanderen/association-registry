namespace AssociationRegistry.Test.Public.Api.Mapping.When_Mapping_A_Location_To_Adres;

using AssociationRegistry.Public.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Public.Api.Verenigingen.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using FluentAssertions;
using Framework;
using Moq;
using System.Linq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Null_Adres
{
    [Fact]
    public void Then_Adres_Is_Null()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var publiekVerenigingDetailDocument = fixture.Create<PubliekVerenigingDetailDocument>();
        publiekVerenigingDetailDocument.Locaties.First().Adres = null;
        var responseMapper = new PubliekVerenigingDetailMapper(new AppSettings(), null);
        var publiekVerenigingDetailResponse = responseMapper.Map(publiekVerenigingDetailDocument, Mock.Of<INamenVoorLidmaatschapMapper>());
        publiekVerenigingDetailResponse.Vereniging.Locaties.First().Adres.Should().BeNull();
    }
}
