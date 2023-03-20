namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Projections.Detail;
using AssociationRegistry.Admin.Api.Projections.Historiek;
using AutoFixture;
using Events;
using Events.CommonEventDataTypes;
using FluentAssertions;
using Framework;
using Framework.Helpers;
using Xunit;
using Xunit.Categories;

[UnitTest]
public abstract class Given_An_Event<TEvent> where TEvent : notnull
{
    protected readonly TestEvent<TEvent> Event;
    private readonly BeheerVerenigingHistoriekDocument _document;
    private readonly BeheerVerenigingHistoriekDocument _documentAfterChanges;

    protected Given_An_Event()
    {
        var fixture = new Fixture().CustomizeAll();
        _document = fixture.Create<BeheerVerenigingHistoriekDocument>();
        Event = fixture.Create<TestEvent<TEvent>>();

        _documentAfterChanges = _document.Copy();

        new BeheerVerenigingHistoriekProjection().Apply((dynamic)Event, _documentAfterChanges);
    }

    protected void AppendsTheCorrectGebeurtenissen(params string[] appendedGebeurtenissen)
    {
        _documentAfterChanges.Should().BeEquivalentTo(
            new BeheerVerenigingHistoriekDocument
            {
                VCode = _document.VCode,
                Gebeurtenissen = _document.Gebeurtenissen.Concat(
                    appendedGebeurtenissen.Select(
                        gebeurtenis => new BeheerVerenigingHistoriekGebeurtenis(
                            gebeurtenis,
                            Event.Initiator,
                            Event.Tijdstip.ToBelgianDateAndTime()))).ToList(),
                Metadata = new Metadata(Event.Sequence, Event.Version),
            }
        );
    }
}

public class Given_A_KorteNaamWerdGewijzigd_Event : Given_An_Event<KorteNaamWerdGewijzigd>
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
        => AppendsTheCorrectGebeurtenissen(
            $"Korte naam vereniging werd gewijzigd naar '{Event.Data.KorteNaam}' door {Event.Initiator} op datum {Event.Tijdstip.ToBelgianDateAndTime()}.");
}

public class Given_A_NaamWerdGewijzigd_Event : Given_An_Event<NaamWerdGewijzigd>
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
        => AppendsTheCorrectGebeurtenissen(
            $"Naam vereniging werd gewijzigd naar '{Event.Data.Naam}' door {Event.Initiator} op datum {Event.Tijdstip.ToBelgianDateAndTime()}.");
}

public class Given_A_KorteBeschrijvingWerdGewijzigd_Event : Given_An_Event<KorteBeschrijvingWerdGewijzigd>
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
        => AppendsTheCorrectGebeurtenissen(
            $"Korte beschrijving vereniging werd gewijzigd naar '{Event.Data.KorteBeschrijving}' door {Event.Initiator} op datum {Event.Tijdstip.ToBelgianDateAndTime()}.");
}

public class Given_A_StartdatumWerdGewijzigd_Event : Given_An_Event<StartdatumWerdGewijzigd>
{
    [Fact]
    public void Test()
        => AppendsTheCorrectGebeurtenissen(
            $"Startdatum vereniging werd gewijzigd naar '{Event.Data.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}' door {Event.Initiator} op datum {Event.Tijdstip.ToBelgianDateAndTime()}.");
}

[UnitTest]
public class Given_A_ContactInfoLijstWerdGewijzigd_Event
{
    private readonly BeheerVerenigingHistoriekDocument _document;
    private readonly BeheerVerenigingHistoriekDocument _documentAfterChanges;
    private readonly TestEvent<ContactInfoLijstWerdGewijzigd> _testEvent;
    private readonly Fixture _fixture;

    public Given_A_ContactInfoLijstWerdGewijzigd_Event()
    {
        _fixture = new Fixture().CustomizeAll();
        _document = _fixture.Create<BeheerVerenigingHistoriekDocument>();
        _testEvent = _fixture.Create<TestEvent<ContactInfoLijstWerdGewijzigd>>();
        _documentAfterChanges = _document.Copy();
    }

    [Fact]
    public void Then_it_adds_a_new_gebeurtenis_for_each_toevoeging()
    {
        _testEvent.Data = new ContactInfoLijstWerdGewijzigd(
            _fixture.Create<string>(),
            new[] { _fixture.Create<ContactInfo>(), _fixture.Create<ContactInfo>(), _fixture.Create<ContactInfo>() },
            Array.Empty<ContactInfo>(),
            Array.Empty<ContactInfo>()
        );
        new BeheerVerenigingHistoriekProjection().Apply(_testEvent, _documentAfterChanges);
        _documentAfterChanges.Should().BeEquivalentTo(
            new BeheerVerenigingHistoriekDocument
            {
                VCode = _document.VCode,
                Gebeurtenissen = _document.Gebeurtenissen
                    .Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging werd toegevoegd met naam '{_testEvent.Data.Toevoegingen[0].Contactnaam}' door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    )
                    .Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging werd toegevoegd met naam '{_testEvent.Data.Toevoegingen[1].Contactnaam}' door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    )
                    .Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging werd toegevoegd met naam '{_testEvent.Data.Toevoegingen[2].Contactnaam}' door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    )
                    .ToList(),
                Metadata = new Metadata(_testEvent.Sequence, _testEvent.Version),
            }
        );
    }

    [Fact]
    public void Then_it_adds_a_new_gebeurtenis_for_each_verwijdering()
    {
        _testEvent.Data = new ContactInfoLijstWerdGewijzigd(
            _fixture.Create<string>(),
            Array.Empty<ContactInfo>(),
            new[] { _fixture.Create<ContactInfo>(), _fixture.Create<ContactInfo>(), _fixture.Create<ContactInfo>() },
            Array.Empty<ContactInfo>()
        );
        new BeheerVerenigingHistoriekProjection().Apply(_testEvent, _documentAfterChanges);
        _documentAfterChanges.Should().BeEquivalentTo(
            new BeheerVerenigingHistoriekDocument
            {
                VCode = _document.VCode,
                Gebeurtenissen = _document.Gebeurtenissen
                    .Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging werd verwijderd met naam '{_testEvent.Data.Verwijderingen[0].Contactnaam}' door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    )
                    .Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging werd verwijderd met naam '{_testEvent.Data.Verwijderingen[1].Contactnaam}' door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    )
                    .Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging werd verwijderd met naam '{_testEvent.Data.Verwijderingen[2].Contactnaam}' door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    )
                    .ToList(),
                Metadata = new Metadata(_testEvent.Sequence, _testEvent.Version),
            }
        );
    }

    [Fact]
    public void Then_it_adds_a_new_gebeurtenis_for_each_property_in_each_wijziging()
    {
        _testEvent.Data = new ContactInfoLijstWerdGewijzigd(
            _fixture.Create<string>(),
            Array.Empty<ContactInfo>(),
            Array.Empty<ContactInfo>(),
            new[] { _fixture.Create<ContactInfo>() with { PrimairContactInfo = true } }
        );
        new BeheerVerenigingHistoriekProjection().Apply(_testEvent, _documentAfterChanges);
        _documentAfterChanges.Should().BeEquivalentTo(
            new BeheerVerenigingHistoriekDocument
            {
                VCode = _document.VCode,
                Gebeurtenissen = _document.Gebeurtenissen
                    .Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging met naam '{_testEvent.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Email' werd gewijzgid naar '{_testEvent.Data.Wijzigingen[0].Email}', door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    ).Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging met naam '{_testEvent.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Telefoon' werd gewijzgid naar '{_testEvent.Data.Wijzigingen[0].Telefoon}', door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    ).Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging met naam '{_testEvent.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Website' werd gewijzgid naar '{_testEvent.Data.Wijzigingen[0].Website}', door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    ).Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging met naam '{_testEvent.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'SocialMedia' werd gewijzgid naar '{_testEvent.Data.Wijzigingen[0].SocialMedia}', door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    ).Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging met naam '{_testEvent.Data.Wijzigingen[0].Contactnaam}' werd als primair aangeduid door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    )
                    .ToList(),
                Metadata = new Metadata(_testEvent.Sequence, _testEvent.Version),
            }
        );
    }

    [Fact]
    public void Then_it_does_not_add_a_new_gebeurtenis_for_each_null_property_in_each_wijziging()
    {
        _testEvent.Data = new ContactInfoLijstWerdGewijzigd(
            _fixture.Create<string>(),
            Array.Empty<ContactInfo>(),
            Array.Empty<ContactInfo>(),
            new[]
            {
                _fixture.Create<ContactInfo>() with { Telefoon = null, Website = null, SocialMedia = null, PrimairContactInfo = false },
                _fixture.Create<ContactInfo>() with { Email = null, Website = null, SocialMedia = null, PrimairContactInfo = false },
                _fixture.Create<ContactInfo>() with { Email = null, Telefoon = null, SocialMedia = null, PrimairContactInfo = false },
                _fixture.Create<ContactInfo>() with { Email = null, Telefoon = null, Website = null, PrimairContactInfo = false },
                _fixture.Create<ContactInfo>() with { Email = null, Telefoon = null, Website = null, SocialMedia = null, PrimairContactInfo = true },
            }
        );
        new BeheerVerenigingHistoriekProjection().Apply(_testEvent, _documentAfterChanges);
        _documentAfterChanges.Should().BeEquivalentTo(
            new BeheerVerenigingHistoriekDocument
            {
                VCode = _document.VCode,
                Gebeurtenissen = _document.Gebeurtenissen
                    .Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging met naam '{_testEvent.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Email' werd gewijzgid naar '{_testEvent.Data.Wijzigingen[0].Email}', door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    ).Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging met naam '{_testEvent.Data.Wijzigingen[1].Contactnaam}' werd gewijzigd, 'Telefoon' werd gewijzgid naar '{_testEvent.Data.Wijzigingen[1].Telefoon}', door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    ).Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging met naam '{_testEvent.Data.Wijzigingen[2].Contactnaam}' werd gewijzigd, 'Website' werd gewijzgid naar '{_testEvent.Data.Wijzigingen[2].Website}', door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    ).Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging met naam '{_testEvent.Data.Wijzigingen[3].Contactnaam}' werd gewijzigd, 'SocialMedia' werd gewijzgid naar '{_testEvent.Data.Wijzigingen[3].SocialMedia}', door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    )
                    .Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging met naam '{_testEvent.Data.Wijzigingen[4].Contactnaam}' werd als primair aangeduid door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    )
                    .ToList(),
                Metadata = new Metadata(_testEvent.Sequence, _testEvent.Version),
            }
        );
    }
}
