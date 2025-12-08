namespace AssociationRegistry.Magda.Persoon;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Events;
using Framework;

public interface IMagdaGeefPersoonService
{
    Task<PersonenUitKsz> GeefPersonen(GeefPersoonRequest[] vertegenwoordigers, CommandMetadata metadata, CancellationToken cancellationToken);
    Task<PersoonUitKsz> GeefPersoon(GeefPersoonRequest vertegenwoordiger, CommandMetadata metadata, CancellationToken cancellationToken);
}


public record GeefPersoonRequest(string Insz, string Voornaam, string Achternaam)
{
    public static GeefPersoonRequest From(Registratiedata.Vertegenwoordiger vertegenwoordiger)
        => new(vertegenwoordiger.Insz, vertegenwoordiger.Voornaam, vertegenwoordiger.Achternaam);
    public static GeefPersoonRequest From(Vertegenwoordiger vertegenwoordiger)
        => new(vertegenwoordiger.Insz, vertegenwoordiger.Voornaam, vertegenwoordiger.Achternaam);
};
