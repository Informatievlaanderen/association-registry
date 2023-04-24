namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Events;
using AutoFixture;
using FluentAssertions;
using Framework;
using Xunit;

public class Given_HoofdactiviteitenVerenigingsregisterWerdenGewijzigd
{
    [Fact]
    public void Then_it_replaces_the_hoofdactiviteitenVerenigingsLoket()
    {
        var fixture = new Fixture().CustomizeAll();
        var hoofdactiviteitenVerenigingsloket = fixture.Create<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>();

        var projectEventOnDetailDocument =
            When<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>
                .Applying(_ => hoofdactiviteitenVerenigingsloket)
                .ToDetailProjectie();

        projectEventOnDetailDocument.HoofdactiviteitenVerenigingsloket.Should()
            .BeEquivalentTo(hoofdactiviteitenVerenigingsloket.HoofdactiviteitenVerenigingsloket);
    }
}
