namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Projections.Historiek;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Historiek;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
{
    private readonly BeheerVerenigingHistoriekDocument _document;
    private readonly TestEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> _verenigingMetRechtspersoonlijkheidWerdGeregistreerd;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd()
    {
        var fixture = new Fixture().CustomizeAll();
        _verenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<TestEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>>();

        _document = BeheerVerenigingHistoriekProjector.Create(_verenigingMetRechtspersoonlijkheidWerdGeregistreerd);
    }

    [Fact]
    public void Then_it_creates_a_new_document()
    {
        _document.Should().BeEquivalentTo(
            new BeheerVerenigingHistoriekDocument
            {
                VCode = _verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                Gebeurtenissen = new List<BeheerVerenigingHistoriekGebeurtenis>
                {
                    new(
                        $"Vereniging met rechtspersoonlijkheid werd geregistreerd met naam '{_verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Naam}'.",
                        nameof(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd),
                        _verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data,
                        _verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Initiator,
                        _verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Tijdstip.ToBelgianDateAndTime()),
                },
                Metadata = new Metadata(_verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Sequence, _verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Version),
            }
        );
    }
}
