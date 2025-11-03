namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Emails;
using Magda.Kbo;
using SocialMedias;
using TelefoonNummers;

public record Vertegenwoordiger
{
    private Vertegenwoordiger(
        bool isPrimair)
    {
        IsPrimair = isPrimair;
    }

    public int VertegenwoordigerId { get; set; }
    public bool IsPrimair { get; init; }

    public Insz Insz { get; init; }
    public string? Roepnaam { get; }
    public string? Rol { get; }
    public Voornaam Voornaam { get; }
    public Achternaam Achternaam { get; }
    public Email Email { get; }
    public TelefoonNummer Telefoon { get; }
    public TelefoonNummer Mobiel { get; }
    public SocialMedia SocialMedia { get; }

    public static Vertegenwoordiger Create(
        bool primairContactpersoon)
        => new(primairContactpersoon);

    public static Vertegenwoordiger Hydrate(
        int vertegenwoordigerId,
        bool isPrimair)
        => new(
            isPrimair)
        {
            VertegenwoordigerId = vertegenwoordigerId,
        };

    public bool WouldBeEquivalent(
        bool? isPrimair,
        out Vertegenwoordiger updatedVertegenwoordiger)
    {
        updatedVertegenwoordiger = CopyWithValuesIfNotNull(isPrimair);

        return this == updatedVertegenwoordiger;
    }

    public bool WouldBeEquivalent(Vertegenwoordiger vertegenwoordiger)
        => this == vertegenwoordiger with { VertegenwoordigerId = VertegenwoordigerId };

    private Vertegenwoordiger CopyWithValuesIfNotNull(
        bool? isPrimair)
        => Create(
                isPrimair ?? IsPrimair) with
            {
                VertegenwoordigerId = VertegenwoordigerId,
            };

    public static Vertegenwoordiger CreateFromKbo(VertegenwoordigerVolgensKbo vertegenwoordigerVolgensKbo)
        => Create(
            false);
}
