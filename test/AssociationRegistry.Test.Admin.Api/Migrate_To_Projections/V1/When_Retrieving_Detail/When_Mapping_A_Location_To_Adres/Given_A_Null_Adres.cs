namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Detail.When_Mapping_A_Location_To_Adres;

using AssociationRegistry.Admin.Api.Verenigingen.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
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
        var fixture = new Fixture().CustomizeAdminApi();
        var beheerVerenigingDetailDocument = fixture.Create<BeheerVerenigingDetailDocument>();
        beheerVerenigingDetailDocument.Locaties.First().Adres = null;

        var beheerVerenigingDetailResponse = new BeheerVerenigingDetailMapper(new AppSettings(),
                                                                              Mock.Of<INamenVoorLidmaatschapMapper>(), null)
           .Map(beheerVerenigingDetailDocument);

        beheerVerenigingDetailResponse.Vereniging.Locaties.First().Adres.Should().BeNull();
    }
}
