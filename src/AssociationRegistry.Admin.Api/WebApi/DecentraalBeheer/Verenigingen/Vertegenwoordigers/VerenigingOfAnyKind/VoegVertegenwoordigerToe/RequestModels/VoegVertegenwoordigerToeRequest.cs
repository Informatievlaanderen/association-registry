namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe.RequestModels;

using AssociationRegistry.Vereniging;
using CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VoegVertegenwoordigerToe;
using Common;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Emails;
using DecentraalBeheer.Vereniging.SocialMedias;
using DecentraalBeheer.Vereniging.TelefoonNummers;
using System.Runtime.Serialization;

[DataContract]
public class VoegVertegenwoordigerToeRequest
{
    /// <summary>De toe te voegen vertegenwoordiger</summary>
    [DataMember(Name = "vertegenwoordiger")]
    public ToeTeVoegenVertegenwoordiger Vertegenwoordiger { get; set; } = null!;

    public VoegVertegenwoordigerToeCommand ToCommand(string vCode)
        => new(
            VCode.Create(vCode),
            Vertegenwoordiger);
    // public record ToeTeVoegenVertegenwoordiger
    // {
    //     private ToeTeVoegenVertegenwoordiger(
    //         Insz insz,
    //         bool isPrimair,
    //         string roepnaam,
    //         string rol,
    //         Voornaam voornaam,
    //         Achternaam achternaam,
    //         Email email,
    //         TelefoonNummer telefoon,
    //         TelefoonNummer mobiel,
    //         SocialMedia socialMedia)
    //     {
    //         Insz = insz;
    //         Voornaam = voornaam;
    //         Achternaam = achternaam;
    //         Email = email;
    //         Telefoon = telefoon;
    //         Mobiel = mobiel;
    //         SocialMedia = socialMedia;
    //     }
    //
    //     public static ToeTeVoegenVertegenwoordiger Create(Insz insz, Voornaam voornaam, Achternaam achternaam, Email email, TelefoonNummer telefoon, TelefoonNummer mobiel, SocialMedia socialMedia)
    //     {
    //         return new ToeTeVoegenVertegenwoordiger(insz, voornaam, achternaam, email, telefoon, mobiel, socialMedia);
    //     }
    //
    //     public int VertegenwoordigerId { get; set; }
    //     public bool IsPrimair { get; init; }
    //
    //     public Insz Insz { get; init; }
    //     public string? Roepnaam { get; }
    //     public string? Rol { get; }
    //     public Voornaam Voornaam { get; }
    //     public Achternaam Achternaam { get; }
    //     public Email Email { get; }
    //     public TelefoonNummer Telefoon { get; }
    //     public TelefoonNummer Mobiel { get; }
    //     public SocialMedia SocialMedia { get; }
    //
    // }
}
