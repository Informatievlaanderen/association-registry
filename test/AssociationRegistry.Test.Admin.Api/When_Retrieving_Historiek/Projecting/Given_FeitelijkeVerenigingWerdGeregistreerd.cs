namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Projections.Detail;
using AssociationRegistry.Admin.Api.Projections.Historiek;
using AssociationRegistry.Admin.Api.Projections.Historiek.Schema;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_FeitelijkeVerenigingWerdGeregistreerd
{
    [Fact]
    public void Then_it_creates_a_new_document()
    {
        var fixture = new Fixture().CustomizeAll();
        var projection = new BeheerVerenigingHistoriekProjection();
        var feitelijkeVerenigingWerdGeregistreerd = fixture.Create<TestEvent<FeitelijkeVerenigingWerdGeregistreerd>>();

        var document = projection.Create(feitelijkeVerenigingWerdGeregistreerd);

        document.Should().BeEquivalentTo(
            new BeheerVerenigingHistoriekDocument
            {
                VCode = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                Gebeurtenissen = new List<BeheerVerenigingHistoriekGebeurtenis>
                {
                    new(
                        $"Feitelijke vereniging werd geregistreerd met naam '{feitelijkeVerenigingWerdGeregistreerd.Data.Naam}'.",
                        nameof(FeitelijkeVerenigingWerdGeregistreerd),
                        feitelijkeVerenigingWerdGeregistreerd.Data,
                        feitelijkeVerenigingWerdGeregistreerd.Initiator,
                        feitelijkeVerenigingWerdGeregistreerd.Tijdstip.ToBelgianDateAndTime()),
                },
                Metadata = new Metadata(feitelijkeVerenigingWerdGeregistreerd.Sequence, feitelijkeVerenigingWerdGeregistreerd.Version),
            }
        );
    }
}
