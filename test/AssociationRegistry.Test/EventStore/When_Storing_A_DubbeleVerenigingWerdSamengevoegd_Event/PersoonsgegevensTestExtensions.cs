namespace AssociationRegistry.Test.When_Storing_A_DubbeleVerenigingWerdSamengevoegd_Event;

using AssociationRegistry.Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Events;
using Persoonsgegevens;

public static class PersoonsgegevensTestExtensions
{
    public static Registratiedata.Vertegenwoordiger Anonymized(this Registratiedata.Vertegenwoordiger v) =>
        v with
        {
            Insz = WellKnownAnonymousFields.Geanonimiseerd,
            Roepnaam = WellKnownAnonymousFields.Geanonimiseerd,
            Rol = WellKnownAnonymousFields.Geanonimiseerd,
            Voornaam = WellKnownAnonymousFields.Geanonimiseerd,
            Achternaam = WellKnownAnonymousFields.Geanonimiseerd,
            Email = WellKnownAnonymousFields.Geanonimiseerd,
            Telefoon = WellKnownAnonymousFields.Geanonimiseerd,
            Mobiel = WellKnownAnonymousFields.Geanonimiseerd,
            SocialMedia = WellKnownAnonymousFields.Geanonimiseerd,
        };

    public static SamenvoegingDubbeleVereniging.Bankrekeningnummer Anonymized(
        this SamenvoegingDubbeleVereniging.Bankrekeningnummer b
    ) => b with { Iban = WellKnownAnonymousFields.Geanonimiseerd, Titularissen = [] };

    public static string[] PersoonsgegevensValues(this Registratiedata.Vertegenwoordiger v) =>
        [v.Insz, v.Roepnaam, v.Rol, v.Voornaam, v.Achternaam, v.Email, v.Telefoon, v.Mobiel, v.SocialMedia];

    public static string[] PersoonsgegevensValues(this SamenvoegingDubbeleVereniging.Bankrekeningnummer b) =>
        [b.Iban, .. b.Titularissen];

    public static VertegenwoordigerPersoonsgegevensDocument ToDocument(
        this Registratiedata.Vertegenwoordiger v,
        string vCode
    ) =>
        new()
        {
            VCode = vCode,
            VertegenwoordigerId = v.VertegenwoordigerId,
            Insz = v.Insz,
            Roepnaam = v.Roepnaam,
            Rol = v.Rol,
            Voornaam = v.Voornaam,
            Achternaam = v.Achternaam,
            Email = v.Email,
            Telefoon = v.Telefoon,
            Mobiel = v.Mobiel,
            SocialMedia = v.SocialMedia,
        };

    public static BankrekeningnummerPersoonsgegevensDocument ToDocument(
        this SamenvoegingDubbeleVereniging.Bankrekeningnummer b,
        string vCode
    ) =>
        new()
        {
            VCode = vCode,
            BankrekeningnummerId = b.BankrekeningnummerId,
            Iban = b.Iban,
            Titularissen = b.Titularissen,
        };
}
