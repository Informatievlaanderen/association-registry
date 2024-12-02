namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Mapping_LocatieIdsPerVCode;

using AssociationRegistry.Grar.LocatieFinder;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_No_Data
{
    [Fact]
    public void Then_Returns_Empty_Array()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var locatieIdsPerVCode = LocatieIdsPerVCodeCollection.Empty;

        var actual = locatieIdsPerVCode.Map(fixture.Create<int>());
        actual.Should().BeEmpty();
    }
}
