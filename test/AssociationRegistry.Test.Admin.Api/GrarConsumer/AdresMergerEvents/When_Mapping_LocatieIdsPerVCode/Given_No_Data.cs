namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.AdresMergerEvents.When_Mapping_LocatieIdsPerVCode;

using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Api.GrarConsumer.Groupers;
using AssociationRegistry.Admin.Api.GrarConsumer.Handlers;
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
        var locatieIdsPerVCode = new LocatieIdsPerVCodeCollection(new Dictionary<string, int[]>());

        var actual = locatieIdsPerVCode.Map(fixture.Create<int>());
        actual.Should().BeEmpty();
    }
}
