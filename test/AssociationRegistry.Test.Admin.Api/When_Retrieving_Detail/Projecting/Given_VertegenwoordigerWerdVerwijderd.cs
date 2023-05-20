namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Projections.Detail;
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
        var fixture = new Fixture().CustomizeAll();
        var vertegenwoordigerWerdVerwijderd = fixture.Create<VertegenwoordigerWerdVerwijderd>();

        var detailDocument = When<VertegenwoordigerWerdVerwijderd>
            .Applying(_ => vertegenwoordigerWerdVerwijderd)
            .ToDetailProjectie(
                d => d with
                {
                    Vertegenwoordigers = d.Vertegenwoordigers.Append(
                        new BeheerVerenigingDetailDocument.Vertegenwoordiger
                        {
                            VertegenwoordigerId = vertegenwoordigerWerdVerwijderd.VertegenwoordigerId,
                        }).ToArray(),
                });

        detailDocument.Vertegenwoordigers.Should().NotContain(
            new BeheerVerenigingDetailDocument.Vertegenwoordiger
            {
                VertegenwoordigerId = vertegenwoordigerWerdVerwijderd.VertegenwoordigerId,
            });
    }
}
