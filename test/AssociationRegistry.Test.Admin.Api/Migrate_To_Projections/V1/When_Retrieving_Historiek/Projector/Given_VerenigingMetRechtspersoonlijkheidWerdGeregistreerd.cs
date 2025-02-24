namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Historiek.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
{
    private readonly BeheerVerenigingHistoriekDocument _document;
    private readonly TestEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> _verenigingMetRechtspersoonlijkheidWerdGeregistreerd;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        _verenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<TestEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>>();

        _document = BeheerVerenigingHistoriekProjector.Create(_verenigingMetRechtspersoonlijkheidWerdGeregistreerd);
    }

    [Fact]
    public void Then_it_creates_a_new_document()
    {
        _document.Gebeurtenissen.Should().BeEquivalentTo(
            new List<BeheerVerenigingHistoriekGebeurtenis>
            {
                new(
                    $"Vereniging met rechtspersoonlijkheid werd geregistreerd met naam '{_verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Naam}'.",
                    nameof(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd),
                    _verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data,
                    _verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Initiator,
                    _verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Tijdstip.FormatAsZuluTime()),
            }
        );
    }
}
