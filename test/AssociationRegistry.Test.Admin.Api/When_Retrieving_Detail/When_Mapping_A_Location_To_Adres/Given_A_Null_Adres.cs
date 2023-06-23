namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.When_Mapping_A_Location_To_Adres;

using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Null_Adres
{
    [Fact]
    public void Then_Adres_Is_Null()
    {
        var fixture = new Fixture().CustomizeAll();
        var beheerVerenigingDetailDocument = fixture.Create<BeheerVerenigingDetailDocument>();
        beheerVerenigingDetailDocument.Locaties.First().Adres = null;
        var beheerVerenigingDetailResponse = new BeheerVerenigingDetailMapper(new AppSettings()).Map(beheerVerenigingDetailDocument);
        beheerVerenigingDetailResponse.Vereniging.Locaties.First().Adres.Should().BeNull();
    }
}
