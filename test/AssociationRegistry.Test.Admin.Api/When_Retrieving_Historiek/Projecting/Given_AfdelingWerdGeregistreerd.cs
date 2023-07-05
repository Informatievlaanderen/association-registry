namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_AfdelingWerdGeregistreerd
{
    [Fact]
    public void Then_it_creates_a_new_document()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var afdelingWerdGeregistreerd = fixture.Create<TestEvent<AfdelingWerdGeregistreerd>>();

        var document = BeheerVerenigingHistoriekProjector.Create(afdelingWerdGeregistreerd);

        document.Should().BeEquivalentTo(
            new BeheerVerenigingHistoriekDocument
            {
                VCode = afdelingWerdGeregistreerd.Data.VCode,
                Gebeurtenissen = new List<BeheerVerenigingHistoriekGebeurtenis>
                {
                    new(
                        $"Afdeling werd geregistreerd met naam '{afdelingWerdGeregistreerd.Data.Naam}'.",
                        nameof(AfdelingWerdGeregistreerd),
                        AfdelingWerdGeregistreerdData.Create(afdelingWerdGeregistreerd.Data),
                        afdelingWerdGeregistreerd.Initiator,
                        afdelingWerdGeregistreerd.Tijdstip.ToBelgianDateAndTime()),
                },
                Metadata = new Metadata(afdelingWerdGeregistreerd.Sequence, afdelingWerdGeregistreerd.Version),
            }
        );
    }
}
