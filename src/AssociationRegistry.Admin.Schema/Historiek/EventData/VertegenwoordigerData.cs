namespace AssociationRegistry.Admin.Schema.Historiek.EventData;

using System.Runtime.Serialization;
using AssociationRegistry.Persoonsgegevens;
using Events;

public interface IHasVertegenwoordigers
{
    VertegenwoordigerData[] Vertegenwoordigers { get; }
    VerenigingWerdGeregistreerdData MakeAnonymous(int vertegenwoordigerId);
}

public interface IHasVertegenwoordigerId
{
    int VertegenwoordigerId { get; }
}

public interface IVertegenwoordigerData<T> : IHasVertegenwoordigerId
{
    public T MakeAnonymous();
}

[DataContract]
public record VertegenwoordigerData(
    [property: DataMember] int VertegenwoordigerId,
    [property: DataMember] bool IsPrimair,
    [property: DataMember] string Roepnaam,
    [property: DataMember] string Rol,
    [property: DataMember] string Voornaam,
    [property: DataMember] string Achternaam,
    [property: DataMember(Name = "E-mail")] string Email,
    [property: DataMember] string Telefoon,
    [property: DataMember] string Mobiel,
    [property: DataMember] string SocialMedia
) : IVertegenwoordigerData<VertegenwoordigerData>
{
    public static VertegenwoordigerData Create(VertegenwoordigerWerdToegevoegd e) =>
        new(
            e.VertegenwoordigerId,
            e.IsPrimair,
            e.Roepnaam,
            e.Rol,
            e.Voornaam,
            e.Achternaam,
            e.Email,
            e.Telefoon,
            e.Mobiel,
            e.SocialMedia
        );

    public static VertegenwoordigerData Create(VertegenwoordigerWerdGewijzigd e) =>
        new(
            e.VertegenwoordigerId,
            e.IsPrimair,
            e.Roepnaam,
            e.Rol,
            e.Voornaam,
            e.Achternaam,
            e.Email,
            e.Telefoon,
            e.Mobiel,
            e.SocialMedia
        );

    public static VertegenwoordigerData Create(Registratiedata.Vertegenwoordiger vertegenwoordiger) =>
        new(
            vertegenwoordiger.VertegenwoordigerId,
            vertegenwoordiger.IsPrimair,
            vertegenwoordiger.Roepnaam,
            vertegenwoordiger.Rol,
            vertegenwoordiger.Voornaam,
            vertegenwoordiger.Achternaam,
            vertegenwoordiger.Email,
            vertegenwoordiger.Telefoon,
            vertegenwoordiger.Mobiel,
            vertegenwoordiger.SocialMedia
        );

    public VertegenwoordigerData MakeAnonymous() =>
        this with
        {
            Achternaam = WellKnownAnonymousFields.Geanonimiseerd,
            Voornaam = WellKnownAnonymousFields.Geanonimiseerd,
            Email = WellKnownAnonymousFields.Geanonimiseerd,
            Mobiel = WellKnownAnonymousFields.Geanonimiseerd,
            Roepnaam = WellKnownAnonymousFields.Geanonimiseerd,
            Rol = WellKnownAnonymousFields.Geanonimiseerd,
            SocialMedia = WellKnownAnonymousFields.Geanonimiseerd,
            Telefoon = WellKnownAnonymousFields.Geanonimiseerd,
        };
}
