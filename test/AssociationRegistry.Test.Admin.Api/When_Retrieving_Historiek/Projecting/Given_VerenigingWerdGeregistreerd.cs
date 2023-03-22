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
public class Given_VerenigingWerdGeregistreerd
{
    private readonly BeheerVerenigingHistoriekDocument _document;
    private readonly TestEvent<VerenigingWerdGeregistreerd> _verenigingWerdGeregistreerd;

    public Given_VerenigingWerdGeregistreerd()
    {
        var fixture = new Fixture().CustomizeAll();
        var beheerVerenigingHistoriekProjection = new BeheerVerenigingHistoriekProjection();
        _verenigingWerdGeregistreerd = fixture.Create<TestEvent<VerenigingWerdGeregistreerd>>();

        _document = beheerVerenigingHistoriekProjection.Create(_verenigingWerdGeregistreerd);
    }

    [Fact]
    public void Then_it_creates_a_new_document()
    {
        _document.Should().BeEquivalentTo(
            new BeheerVerenigingHistoriekDocument
            {
                VCode = _verenigingWerdGeregistreerd.Data.VCode,
                Gebeurtenissen = new List<BeheerVerenigingHistoriekGebeurtenis>
                {
                    new(
                        $"Vereniging werd geregistreerd met naam '{_verenigingWerdGeregistreerd.Data.Naam}'.",
                        nameof(VerenigingWerdGeregistreerd),
                        new VerenigingWerdgeregsitreerdData(_verenigingWerdGeregistreerd.Data),
                        _verenigingWerdGeregistreerd.Initiator,
                        _verenigingWerdGeregistreerd.Tijdstip.ToBelgianDateAndTime()),
                },
                Metadata = new Metadata(_verenigingWerdGeregistreerd.Sequence, _verenigingWerdGeregistreerd.Version),
            }
        );
    }
}
