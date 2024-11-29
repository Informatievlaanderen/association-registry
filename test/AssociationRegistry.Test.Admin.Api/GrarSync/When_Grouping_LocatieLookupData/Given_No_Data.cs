namespace AssociationRegistry.Test.Admin.Api.GrarSync.When_Grouping_LocatieLookupData;

using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Api.GrarConsumer.Groupers;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_No_Data
{
    [Fact]
    public void Then_Returns_Empty_Array()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var locatieLookupData = Array.Empty<LocatieLookupData>();
        var actual = LocatiesVolgensVCodeGrouper.Group(locatieLookupData, fixture.Create<int>());
        actual.Should().BeEmpty();
    }
}
