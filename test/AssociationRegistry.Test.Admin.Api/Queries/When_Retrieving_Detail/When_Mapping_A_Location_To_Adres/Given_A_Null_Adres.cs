<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Detail/When_Mapping_A_Location_To_Adres/Given_A_Null_Adres.cs
namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail.When_Mapping_A_Location_To_Adres;
========
namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Detail.When_Mapping_A_Location_To_Adres;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Detail/When_Mapping_A_Location_To_Adres/Given_A_Null_Adres.cs

using AssociationRegistry.Admin.Api.Verenigingen.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
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
        var beheerVerenigingDetailResponse = new BeheerVerenigingDetailMapper(new AppSettings()).Map(beheerVerenigingDetailDocument);
        beheerVerenigingDetailResponse.Vereniging.Locaties.First().Adres.Should().BeNull();
    }
}
