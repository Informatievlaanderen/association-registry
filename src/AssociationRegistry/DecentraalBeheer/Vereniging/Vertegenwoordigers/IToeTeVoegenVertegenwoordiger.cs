namespace AssociationRegistry.DecentraalBeheer.Vereniging;

public interface IToeTeVoegenVertegenwoordiger
{
    /// <summary>
    /// Dit is de unieke identificatie van een vertegenwoordiger, dit kan een rijksregisternummer of bisnummer zijn
    /// </summary>
    string Insz { get; set; }

    /// <summary>De voornaam van de vertegenwoordiger</summary>
    string Voornaam { get; set; }

    /// <summary>De achternaam van de vertegenwoordiger</summary>
    string Achternaam { get; set; }

    /// <summary>Dit is de rol van de vertegenwoordiger binnen de vereniging</summary>
    string? Rol { get; set; }

    /// <summary>Dit is de roepnaam van de vertegenwoordiger</summary>
    string? Roepnaam { get; set; }

    /// <summary>
    ///     Dit duidt aan dat dit de unieke primaire contactpersoon is voor alle communicatie met overheidsinstanties
    /// </summary>
    bool IsPrimair { get; set; }

    /// <summary>Het e-mailadres van de vertegenwoordiger</summary>
    string? Email { get; set; }

    /// <summary>Het telefoonnummer van de vertegenwoordiger</summary>
    string? Telefoon { get; set; }

    /// <summary>Het mobiel nummer van de vertegenwoordiger</summary>
    string? Mobiel { get; set; }

    /// <summary>Het socialmedia account van de vertegenwoordiger</summary>
    string? SocialMedia { get; set; }
}
