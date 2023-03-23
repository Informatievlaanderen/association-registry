namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Projections.Historiek.Schema;
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
                        Toevoegingen = _fixture.CreateMany<ContactInfo>(3).ToArray(),
                        Verwijderingen = Array.Empty<ContactInfo>(),
                        Wijzigingen = Array.Empty<ContactInfo>(),
                    });


        projectEventOnHistoriekDocument.AppendsTheCorrectGebeurtenissen(
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Toevoegingen[0].Contactnaam}' werd toegevoegd.",
                nameof(ContactInfoLijstWerdGewijzigd),
                new ContactInfoWerdToegevoegdData(projectEventOnHistoriekDocument.Event.Data.Toevoegingen[0]),
                initiator,
                tijdstip),
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Toevoegingen[1].Contactnaam}' werd toegevoegd.",
                nameof(ContactInfoLijstWerdGewijzigd),
                new ContactInfoWerdToegevoegdData(projectEventOnHistoriekDocument.Event.Data.Toevoegingen[1]),
                initiator,
                tijdstip),
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Toevoegingen[2].Contactnaam}' werd toegevoegd.",
                nameof(ContactInfoLijstWerdGewijzigd),
                new ContactInfoWerdToegevoegdData(projectEventOnHistoriekDocument.Event.Data.Toevoegingen[2]),
                initiator,
                tijdstip)
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
                        Verwijderingen = _fixture.CreateMany<ContactInfo>(3).ToArray(),
                        Wijzigingen = Array.Empty<ContactInfo>(),
                    });

        projectEventOnHistoriekDocument.AppendsTheCorrectGebeurtenissen(
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Verwijderingen[0].Contactnaam}' werd verwijderd.",
                nameof(ContactInfoLijstWerdGewijzigd),
                new ContactInfoWerdVerwijderdData(projectEventOnHistoriekDocument.Event.Data.Verwijderingen[0].Contactnaam),
                initiator,
                tijdstip),
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Verwijderingen[1].Contactnaam}' werd verwijderd.",
                nameof(ContactInfoLijstWerdGewijzigd),
                new ContactInfoWerdVerwijderdData(projectEventOnHistoriekDocument.Event.Data.Verwijderingen[1].Contactnaam),
                initiator,
                tijdstip),
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Verwijderingen[2].Contactnaam}' werd verwijderd.",
                nameof(ContactInfoLijstWerdGewijzigd),
                new ContactInfoWerdVerwijderdData(projectEventOnHistoriekDocument.Event.Data.Verwijderingen[2].Contactnaam),
                initiator,
                tijdstip)
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
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Email' werd gewijzigd naar '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Email}'.",
                nameof(ContactInfoLijstWerdGewijzigd),
                new EmailContactInfoWerdGewijzigdHistoriekData(projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam, projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Email!),
                initiator,
                tijdstip),
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Telefoon' werd gewijzigd naar '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Telefoon}'.",
                nameof(ContactInfoLijstWerdGewijzigd),
                new TelefoonContactInfoWerdGewijzigdHistoriekData(projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam, projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Telefoon!),
                initiator,
                tijdstip),
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Website' werd gewijzigd naar '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Website}'.",
                nameof(ContactInfoLijstWerdGewijzigd),
                new WebsiteContactInfoWerdGewijzigdHistoriekData(projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam, projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Website!),
                initiator,
                tijdstip),
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'SocialMedia' werd gewijzigd naar '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].SocialMedia}'.",
                nameof(ContactInfoLijstWerdGewijzigd),
                new SocialMediaContactInfoWerdGewijzigdHistoriekData(projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam, projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].SocialMedia!),
                initiator,
                tijdstip),
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd als primair aangeduid.",
                nameof(ContactInfoLijstWerdGewijzigd),
                new PrimairContactInfoWerdGewijzigdHistoriekData(projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam, projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].PrimairContactInfo),
                initiator,
                tijdstip)
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
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Verwijderingen[0].Contactnaam}' werd verwijderd.",
                nameof(ContactInfoLijstWerdGewijzigd),
                new ContactInfoWerdVerwijderdData(projectEventOnHistoriekDocument.Event.Data.Verwijderingen[0].Contactnaam),
                initiator,
                tijdstip),
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Email' werd gewijzigd naar '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Email}'.",
                nameof(ContactInfoLijstWerdGewijzigd),
                new EmailContactInfoWerdGewijzigdHistoriekData(projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam, projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Email!),
                initiator,
                tijdstip),
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Telefoon' werd gewijzigd naar '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Telefoon}'.",
                nameof(ContactInfoLijstWerdGewijzigd),
                new TelefoonContactInfoWerdGewijzigdHistoriekData(projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam, projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Telefoon!),
                initiator,
                tijdstip),
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'Website' werd gewijzigd naar '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Website}'.",
                nameof(ContactInfoLijstWerdGewijzigd),
                new WebsiteContactInfoWerdGewijzigdHistoriekData(projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam, projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Website!),
                initiator,
                tijdstip),
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd gewijzigd, 'SocialMedia' werd gewijzigd naar '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].SocialMedia}'.",
                nameof(ContactInfoLijstWerdGewijzigd),
                new SocialMediaContactInfoWerdGewijzigdHistoriekData(projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam, projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].SocialMedia!),
                initiator,
                tijdstip),
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam}' werd als primair aangeduid.",
                nameof(ContactInfoLijstWerdGewijzigd),
                new PrimairContactInfoWerdGewijzigdHistoriekData(projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].Contactnaam, projectEventOnHistoriekDocument.Event.Data.Wijzigingen[0].PrimairContactInfo),
                initiator,
                tijdstip),
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactinfo met naam '{projectEventOnHistoriekDocument.Event.Data.Toevoegingen[0].Contactnaam}' werd toegevoegd.",
                nameof(ContactInfoLijstWerdGewijzigd),
                new ContactInfoWerdToegevoegdData(projectEventOnHistoriekDocument.Event.Data.Toevoegingen[0]),
                initiator,
                tijdstip)
        );
    }
}
