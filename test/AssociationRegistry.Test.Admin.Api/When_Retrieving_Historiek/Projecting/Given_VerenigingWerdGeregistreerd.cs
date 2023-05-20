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
    private readonly BeheerVerenigingHistoriekDocument _document;
    private readonly TestEvent<FeitelijkeVerenigingWerdGeregistreerd> _feitelijkeVerenigingWerdGeregistreerd;

    public Given_FeitelijkeVerenigingWerdGeregistreerd()
    {
        var fixture = new Fixture().CustomizeAll();
        var beheerVerenigingHistoriekProjection = new BeheerVerenigingHistoriekProjection();
        _feitelijkeVerenigingWerdGeregistreerd = fixture.Create<TestEvent<FeitelijkeVerenigingWerdGeregistreerd>>();

        _document = beheerVerenigingHistoriekProjection.Create(_feitelijkeVerenigingWerdGeregistreerd);
    }

    [Fact]
    public void Then_it_creates_a_new_document()
    {
        _document.Should().BeEquivalentTo(
            new BeheerVerenigingHistoriekDocument
            {
                VCode = _feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                Gebeurtenissen = new List<BeheerVerenigingHistoriekGebeurtenis>
                {
                    new(
                        $"Feitelijke vereniging werd geregistreerd met naam '{_feitelijkeVerenigingWerdGeregistreerd.Data.Naam}'.",
                        nameof(FeitelijkeVerenigingWerdGeregistreerd),
                        _feitelijkeVerenigingWerdGeregistreerd.Data,
                        _feitelijkeVerenigingWerdGeregistreerd.Initiator,
                        _feitelijkeVerenigingWerdGeregistreerd.Tijdstip.ToBelgianDateAndTime()),
                },
                Metadata = new Metadata(_feitelijkeVerenigingWerdGeregistreerd.Sequence, _feitelijkeVerenigingWerdGeregistreerd.Version),
            }
        );
    }
}
