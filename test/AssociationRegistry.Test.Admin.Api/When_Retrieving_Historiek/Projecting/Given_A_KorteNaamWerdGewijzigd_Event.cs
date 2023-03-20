namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Projections.Detail;
using AssociationRegistry.Admin.Api.Projections.Historiek;
using AutoFixture;
using Events;
using FluentAssertions;
using FluentAssertions.Execution;
using Framework;
using Framework.Helpers;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_KorteNaamWerdGewijzigd_Event
{
    private readonly TestEvent<KorteNaamWerdGewijzigd> _korteNaamWerdGewijzigd;
    private readonly BeheerVerenigingHistoriekDocument _document;
    private readonly BeheerVerenigingHistoriekDocument _documentAfterChanges;

    public Given_A_KorteNaamWerdGewijzigd_Event()
    {
        var fixture = new Fixture().CustomizeAll();
        var beheerVerenigingHistoriekProjection = new BeheerVerenigingHistoriekProjection();
        _document = fixture.Create<BeheerVerenigingHistoriekDocument>();
        _korteNaamWerdGewijzigd = fixture.Create<TestEvent<KorteNaamWerdGewijzigd>>();

        _documentAfterChanges = _document.Copy();
        beheerVerenigingHistoriekProjection.Apply(_korteNaamWerdGewijzigd, _documentAfterChanges);
    }

    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        _documentAfterChanges.Should().BeEquivalentTo(
            new BeheerVerenigingHistoriekDocument
            {
                VCode = _document.VCode,
                Gebeurtenissen = _document.Gebeurtenissen.Append(new BeheerVerenigingHistoriekGebeurtenis
                (
                    $"Korte naam vereniging werd gewijzigd van naar '{_korteNaamWerdGewijzigd.Data.KorteNaam}' door {_korteNaamWerdGewijzigd.Initiator} op datum {_korteNaamWerdGewijzigd.Tijdstip.ToBelgianDateAndTime()}",
                        _korteNaamWerdGewijzigd.Initiator,
                        _korteNaamWerdGewijzigd.Tijdstip.ToBelgianDateAndTime()
                )).ToList(),
                Metadata = new Metadata(_korteNaamWerdGewijzigd.Sequence, _korteNaamWerdGewijzigd.Version),
            }
        );
    }
}
