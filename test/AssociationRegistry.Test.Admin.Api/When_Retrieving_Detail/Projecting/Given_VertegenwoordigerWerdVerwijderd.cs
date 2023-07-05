namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema;
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

        var vertegenwoordiger = fixture.Create<BeheerVerenigingDetailDocument.Vertegenwoordiger>() with
        {
            VertegenwoordigerId = vertegenwoordigerWerdVerwijderd.Data.VertegenwoordigerId,
        };

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();
        doc.Vertegenwoordigers = doc.Vertegenwoordigers.Append(
            vertegenwoordiger
        ).ToArray();

        BeheerVerenigingDetailProjector.Apply(vertegenwoordigerWerdVerwijderd, doc);


        doc.Vertegenwoordigers.Should().NotContain(
            new BeheerVerenigingDetailDocument.Vertegenwoordiger
            {
                VertegenwoordigerId = vertegenwoordigerWerdVerwijderd.Data.VertegenwoordigerId,
            });
        doc.Vertegenwoordigers.Should().BeInAscendingOrder(v => v.VertegenwoordigerId);
        doc.DatumLaatsteAanpassing.Should().Be(vertegenwoordigerWerdVerwijderd.Tijdstip.ToBelgianDate());
        doc.Metadata.Should().BeEquivalentTo(new Metadata(vertegenwoordigerWerdVerwijderd.Sequence, vertegenwoordigerWerdVerwijderd.Version));
    }
}
