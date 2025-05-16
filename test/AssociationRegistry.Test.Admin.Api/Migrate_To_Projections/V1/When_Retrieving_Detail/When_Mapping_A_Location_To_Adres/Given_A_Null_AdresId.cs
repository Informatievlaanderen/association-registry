namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Detail.When_Mapping_A_Location_To_Adres;

using AssociationRegistry.Admin.Api.Verenigingen.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Null_AdresId
{
    [Fact]
    public void Then_AdresId_Is_Null()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var beheerVerenigingDetailDocument = fixture.Create<BeheerVerenigingDetailDocument>();
        beheerVerenigingDetailDocument.Locaties.First().AdresId = null;

        var beheerVerenigingDetailResponse = new BeheerVerenigingDetailMapper(new AppSettings(),
                                                                              Mock.Of<IVerplichteNamenVoorVCodesMapper>(), null)
           .Map(beheerVerenigingDetailDocument);

        beheerVerenigingDetailResponse.Vereniging.Locaties.First().AdresId.Should().BeNull();
    }
}
