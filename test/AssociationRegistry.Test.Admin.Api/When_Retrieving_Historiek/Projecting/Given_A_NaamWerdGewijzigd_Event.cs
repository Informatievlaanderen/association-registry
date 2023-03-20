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
public class Given_A_NaamWerdGewijzigd_Event
{
    private readonly TestEvent<NaamWerdGewijzigd> _naamWerdGewijzigd;
    private readonly BeheerVerenigingHistoriekDocument _document;
    private readonly BeheerVerenigingHistoriekDocument _documentAfterChanges;

    public Given_A_NaamWerdGewijzigd_Event()
    {
        var fixture = new Fixture().CustomizeAll();
        var beheerVerenigingHistoriekProjection = new BeheerVerenigingHistoriekProjection();
        _document = fixture.Create<BeheerVerenigingHistoriekDocument>();
        _naamWerdGewijzigd = fixture.Create<TestEvent<NaamWerdGewijzigd>>();

        _documentAfterChanges = _document.Copy();
        beheerVerenigingHistoriekProjection.Apply(_naamWerdGewijzigd, _documentAfterChanges);
    }

    [Fact]
    public void Then_it_creates_a_new_document()
    {
        using (new AssertionScope())
        {
            _documentAfterChanges.Gebeurtenissen.Should().HaveCount(_document.Gebeurtenissen.Count + 1);
            _documentAfterChanges.Gebeurtenissen.Should().EndWith(
                new BeheerVerenigingHistoriekGebeurtenis(
                    $"Naam vereniging werd gewijzigd naar '{_naamWerdGewijzigd.Data.Naam}' door {_naamWerdGewijzigd.Initiator} op datum {_naamWerdGewijzigd.Tijdstip.ToBelgianDateAndTime()}",
                    _naamWerdGewijzigd.Initiator,
                    _naamWerdGewijzigd.Tijdstip.ToBelgianDateAndTime()));
            _documentAfterChanges.Metadata.Should().BeEquivalentTo(new Metadata(_naamWerdGewijzigd.Sequence, _naamWerdGewijzigd.Version));
        }
    }
}
