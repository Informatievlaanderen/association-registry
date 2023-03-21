namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Projections.Historiek;
using AutoFixture;
using Events;
using Events.CommonEventDataTypes;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactInfoLijstWerdGewijzigd
{
    private readonly Fixture _fixture;

    public Given_ContactInfoLijstWerdGewijzigd()
    {
        _fixture = new Fixture().CustomizeAll();
    }

    [Fact]
    public void Then_it_adds_a_new_gebeurtenis_for_each_toevoeging()
    {
        var projectEventOnHistoriekDocument =
            WhenApplying<ContactInfoLijstWerdGewijzigd>
                .ToHistoriekProjectie(
                    e => e with
                    {
                        Toevoegingen = new[] { _fixture.Create<ContactInfo>(), _fixture.Create<ContactInfo>(), _fixture.Create<ContactInfo>() },
                        Verwijderingen = Array.Empty<ContactInfo>(),
                        Wijzigingen = Array.Empty<ContactInfo>(),
                    });


        projectEventOnHistoriekDocument.AppendsTheCorrectGebeurtenissen(
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Toevoegingen[0].Contactnaam}' werd toegevoegd.", nameof(ContactInfoLijstWerdGewijzigd), i, t),
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Toevoegingen[1].Contactnaam}' werd toegevoegd.", nameof(ContactInfoLijstWerdGewijzigd), i, t),
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Toevoegingen[2].Contactnaam}' werd toegevoegd.", nameof(ContactInfoLijstWerdGewijzigd), i, t)
        );
    }

    [Fact]
    public void Then_it_adds_a_new_gebeurtenis_for_each_verwijdering()
    {
        var projectEventOnHistoriekDocument =
            WhenApplying<ContactInfoLijstWerdGewijzigd>
                .ToHistoriekProjectie(
                    e => e with
                    {
                        Toevoegingen = Array.Empty<ContactInfo>(),
                        Verwijderingen = new[] { _fixture.Create<ContactInfo>(), _fixture.Create<ContactInfo>(), _fixture.Create<ContactInfo>() },
                        Wijzigingen = Array.Empty<ContactInfo>(),
                    });

        projectEventOnHistoriekDocument.AppendsTheCorrectGebeurtenissen(
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Verwijderingen[0].Contactnaam}' werd verwijderd.", nameof(ContactInfoLijstWerdGewijzigd), i, t),
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Verwijderingen[1].Contactnaam}' werd verwijderd.", nameof(ContactInfoLijstWerdGewijzigd), i, t),
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Verwijderingen[2].Contactnaam}' werd verwijderd.", nameof(ContactInfoLijstWerdGewijzigd), i, t)
        );
    }

    [Fact]
    public void Then_it_adds_a_new_gebeurtenis_for_each_property_in_each_wijziging()
    {
        var projectEventOnHistoriekDocument =
            WhenApplying<ContactInfoLijstWerdGewijzigd>
                .ToHistoriekProjectie(
                    e => e with
                    {
                        Toevoegingen = Array.Empty<ContactInfo>(),
                        Verwijderingen = Array.Empty<ContactInfo>(),
                        Wijzigingen = new[] { _fixture.Create<ContactInfo>() with { PrimairContactInfo = true } },
                    });

        projectEventOnHistoriekDocument.AppendsTheCorrectGebeurtenissen(
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Email' werd gewijzigd naar '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Email}'.", nameof(ContactInfoLijstWerdGewijzigd), i, t),
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Telefoon' werd gewijzigd naar '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Telefoon}'.", nameof(ContactInfoLijstWerdGewijzigd), i, t),
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Website' werd gewijzigd naar '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Website}'.", nameof(ContactInfoLijstWerdGewijzigd), i, t),
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'SocialMedia' werd gewijzigd naar '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].SocialMedia}'.", nameof(ContactInfoLijstWerdGewijzigd), i, t),
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd als primair aangeduid.", nameof(ContactInfoLijstWerdGewijzigd), i, t)
        );
    }

    [Fact]
    public void Then_it_does_not_add_a_new_gebeurtenis_for_each_null_property_in_each_wijziging()
    {
        var projectEventOnHistoriekDocument =
            WhenApplying<ContactInfoLijstWerdGewijzigd>
                .ToHistoriekProjectie(
                    e => e with
                    {
                        Toevoegingen = Array.Empty<ContactInfo>(),
                        Verwijderingen = Array.Empty<ContactInfo>(),
                        Wijzigingen = new[]
                        {
                            _fixture.Create<ContactInfo>() with { Email = null, Telefoon = null, Website = null, SocialMedia = null, PrimairContactInfo = false },
                        },
                    });
        projectEventOnHistoriekDocument.AppendsTheCorrectGebeurtenissen();
    }

    [Fact]
    public void Then_it_first_adds_verwijderingen_Then_wijzigingen_Then_toevoegingen()
    {
        var projectEventOnHistoriekDocument =
            WhenApplying<ContactInfoLijstWerdGewijzigd>
                .ToHistoriekProjectie(
                    e => e with
                    {
                        Toevoegingen = new[] { _fixture.Create<ContactInfo>() },
                        Verwijderingen = new[] { _fixture.Create<ContactInfo>() },
                        Wijzigingen = new[] { _fixture.Create<ContactInfo>() with { PrimairContactInfo = true } },
                    });

        projectEventOnHistoriekDocument.AppendsTheCorrectGebeurtenissen(
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Verwijderingen[0].Contactnaam}' werd verwijderd.", nameof(ContactInfoLijstWerdGewijzigd), i, t),
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Email' werd gewijzigd naar '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Email}'.", nameof(ContactInfoLijstWerdGewijzigd), i, t),
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Telefoon' werd gewijzigd naar '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Telefoon}'.", nameof(ContactInfoLijstWerdGewijzigd), i, t),
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Website' werd gewijzigd naar '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Website}'.", nameof(ContactInfoLijstWerdGewijzigd), i, t),
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'SocialMedia' werd gewijzigd naar '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].SocialMedia}'.", nameof(ContactInfoLijstWerdGewijzigd), i, t),
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd als primair aangeduid.", nameof(ContactInfoLijstWerdGewijzigd), i, t),
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Toevoegingen[0].Contactnaam}' werd toegevoegd.", nameof(ContactInfoLijstWerdGewijzigd), i, t)
        );
    }
}
