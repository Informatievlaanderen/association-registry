namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VertegenwoordigerWerdVerwijderd
{
    [Fact]
    public void Then_it_removes_the_vertegenwoordiger_from_the_detail()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var vertegenwoordigerWerdVerwijderd = fixture.Create<TestEvent<VertegenwoordigerWerdVerwijderd>>();

        var vertegenwoordiger = fixture.Create<Vertegenwoordiger>() with
        {
            VertegenwoordigerId = vertegenwoordigerWerdVerwijderd.Data.VertegenwoordigerId,
        };

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        doc.Vertegenwoordigers = doc.Vertegenwoordigers.Append(
            vertegenwoordiger
        ).ToArray();

        BeheerVerenigingDetailProjector.Apply(vertegenwoordigerWerdVerwijderd, doc);

        doc.Vertegenwoordigers.Should().NotContain(
            new Vertegenwoordiger
            {
                VertegenwoordigerId = vertegenwoordigerWerdVerwijderd.Data.VertegenwoordigerId,
            });

        doc.Vertegenwoordigers.Should().BeInAscendingOrder(v => v.VertegenwoordigerId);
    }
}
