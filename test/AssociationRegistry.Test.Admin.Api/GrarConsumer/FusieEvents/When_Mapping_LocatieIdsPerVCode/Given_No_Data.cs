namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Mapping_LocatieIdsPerVCode;

using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using CommandHandling.Grar.GrarUpdates.LocatieFinder;
using FluentAssertions;
using Xunit;

public class Given_No_Data
{
    [Fact]
    public void Then_Returns_Empty_Array()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var locatieIdsPerVCode = LocatiesPerVCodeCollection.Empty;

        var actual = locatieIdsPerVCode.Map(fixture.Create<int>(), fixture.Create<string>());
        actual.Should().BeEmpty();
    }
}
