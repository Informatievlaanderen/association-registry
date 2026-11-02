namespace AssociationRegistry.Test.Common.DubbelMerge;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;

/// <summary>
///     Contactgegevens voor de samenvoeging van een dubbele vereniging. De ids zijn die op de eigen vereniging.
///     Het telefoonnummer van Rivendell en dat van Fellowship zijn hetzelfde nummer, anders geschreven.
/// </summary>
public static class TestContactgegevens
{
    public static readonly Registratiedata.Contactgegeven EmailRivendell = new(
        ContactgegevenId: 1,
        Contactgegeventype: Contactgegeventype.Labels.Email,
        Waarde: "info@rivendell.example",
        Beschrijving: "",
        IsPrimair: true
    );

    public static readonly Registratiedata.Contactgegeven TelefoonRivendell = new(
        ContactgegevenId: 2,
        Contactgegeventype: Contactgegeventype.Labels.Telefoon,
        Waarde: "011 22 33 44",
        Beschrijving: "Secretariaat",
        IsPrimair: false
    );

    public static readonly Registratiedata.Contactgegeven EmailFellowship = new(
        ContactgegevenId: 1,
        Contactgegeventype: Contactgegeventype.Labels.Email,
        Waarde: "info@fellowship.example",
        Beschrijving: "",
        IsPrimair: true
    );

    public static readonly Registratiedata.Contactgegeven WebsiteFellowship = new(
        ContactgegevenId: 2,
        Contactgegeventype: Contactgegeventype.Labels.Website,
        Waarde: "https://fellowship.example",
        Beschrijving: "",
        IsPrimair: true
    );

    public static readonly Registratiedata.Contactgegeven TelefoonFellowship = new(
        ContactgegevenId: 3,
        Contactgegeventype: Contactgegeventype.Labels.Telefoon,
        Waarde: "+32 11 22 33 44",
        Beschrijving: "Secretariaat",
        IsPrimair: false
    );

    public static readonly Registratiedata.Contactgegeven SocialMediaFellowship = new(
        ContactgegevenId: 5,
        Contactgegeventype: Contactgegeventype.Labels.SocialMedia,
        Waarde: "https://social.example/fellowship",
        Beschrijving: "",
        IsPrimair: false
    );

    public static readonly Registratiedata.Contactgegeven OngeldigeTelefoonFellowship = new(
        ContactgegevenId: 4,
        Contactgegeventype: Contactgegeventype.Labels.Telefoon,
        Waarde: "geen nummer",
        Beschrijving: "",
        IsPrimair: false
    );
}

public static class TestContactgegevensExtensions
{
    public static FeitelijkeVerenigingWerdGeregistreerd WithContactgegevens(
        this FeitelijkeVerenigingWerdGeregistreerd @event,
        Registratiedata.Contactgegeven[] contactgegevens
    ) => @event with { Contactgegevens = contactgegevens };

    public static VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd WithContactgegevens(
        this VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd @event,
        Registratiedata.Contactgegeven[] contactgegevens
    ) => @event with { Contactgegevens = contactgegevens };
}
