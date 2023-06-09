namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Historiek;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;
using Formatters = AssociationRegistry.Admin.Api.Infrastructure.Extensions.Formatters;

[UnitTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
{
    private readonly BeheerVerenigingHistoriekDocument _document;
    private readonly TestEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> _verenigingMetRechtspersoonlijkheidWerdGeregistreerd;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd()
    {
        var fixture = new Fixture().CustomizeAll();
        var beheerVerenigingHistoriekProjection = new BeheerVerenigingHistoriekProjection();
        _verenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<TestEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>>();

        _document = beheerVerenigingHistoriekProjection.Create(_verenigingMetRechtspersoonlijkheidWerdGeregistreerd);
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
                        Formatters.ToBelgianDateAndTime(_verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Tijdstip)),
                },
                Metadata = new Metadata(_verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Sequence, _verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Version),
            }
        );
    }
}
