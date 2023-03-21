namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

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
public class Given_ContactInfoLijstWerdGewijzigd
{
    private readonly BeheerVerenigingHistoriekDocument _document;
    private readonly BeheerVerenigingHistoriekDocument _documentAfterChanges;
    private readonly TestEvent<ContactInfoLijstWerdGewijzigd> _testEvent;
    private readonly Fixture _fixture;

    public Given_ContactInfoLijstWerdGewijzigd()
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
                            $"Contactinfo vereniging met naam '{_testEvent.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Email' werd gewijzigd naar '{_testEvent.Data.Wijzigingen[0].Email}', door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    ).Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging met naam '{_testEvent.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Telefoon' werd gewijzigd naar '{_testEvent.Data.Wijzigingen[0].Telefoon}', door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    ).Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging met naam '{_testEvent.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Website' werd gewijzigd naar '{_testEvent.Data.Wijzigingen[0].Website}', door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    ).Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging met naam '{_testEvent.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'SocialMedia' werd gewijzigd naar '{_testEvent.Data.Wijzigingen[0].SocialMedia}', door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
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
                _fixture.Create<ContactInfo>() with { Email = null, Telefoon = null, Website = null, SocialMedia = null, PrimairContactInfo = false },
            }
        );
        new BeheerVerenigingHistoriekProjection().Apply(_testEvent, _documentAfterChanges);
        _documentAfterChanges.Should().BeEquivalentTo(
            new BeheerVerenigingHistoriekDocument
            {
                VCode = _document.VCode,
                Gebeurtenissen = _document.Gebeurtenissen,
                Metadata = new Metadata(_testEvent.Sequence, _testEvent.Version),
            }
        );
    }

    [Fact]
    public void Then_it_first_adds_verwijderingen_Then_wijzigingen_Then_toevoegingen()
    {
        _testEvent.Data = new ContactInfoLijstWerdGewijzigd(
            _fixture.Create<string>(),
            new[] { _fixture.Create<ContactInfo>() },
            new[] { _fixture.Create<ContactInfo>() },
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
                            $"Contactinfo vereniging werd verwijderd met naam '{_testEvent.Data.Verwijderingen[0].Contactnaam}' door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    )
                    .Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging met naam '{_testEvent.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Email' werd gewijzigd naar '{_testEvent.Data.Wijzigingen[0].Email}', door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    ).Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging met naam '{_testEvent.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Telefoon' werd gewijzigd naar '{_testEvent.Data.Wijzigingen[0].Telefoon}', door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    ).Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging met naam '{_testEvent.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Website' werd gewijzigd naar '{_testEvent.Data.Wijzigingen[0].Website}', door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    ).Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging met naam '{_testEvent.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'SocialMedia' werd gewijzigd naar '{_testEvent.Data.Wijzigingen[0].SocialMedia}', door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    ).Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging met naam '{_testEvent.Data.Wijzigingen[0].Contactnaam}' werd als primair aangeduid door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    ).Append(
                        new BeheerVerenigingHistoriekGebeurtenis(
                            $"Contactinfo vereniging werd toegevoegd met naam '{_testEvent.Data.Toevoegingen[0].Contactnaam}' door {_testEvent.Initiator} op datum {_testEvent.Tijdstip.ToBelgianDateAndTime()}.",
                            _testEvent.Initiator,
                            _testEvent.Tijdstip.ToBelgianDateAndTime()
                        )
                    )
                    .ToList(),
                Metadata = new Metadata(_testEvent.Sequence, _testEvent.Version),
            }, options => options.WithStrictOrderingFor(doc=>doc.Gebeurtenissen)
        );
    }
}
