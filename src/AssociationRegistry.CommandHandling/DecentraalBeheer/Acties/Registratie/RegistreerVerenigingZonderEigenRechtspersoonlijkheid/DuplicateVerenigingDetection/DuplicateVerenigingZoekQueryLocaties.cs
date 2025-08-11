namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.GemeentenaamVerrijking;
using System.Linq;

public record DuplicateVerenigingZoekQueryLocaties
{
    private readonly Locatie[] _locaties;

    public DuplicateVerenigingZoekQueryLocaties(Locatie[] locaties)
    {
        _locaties = locaties;
    }

    public DuplicateVerenigingZoekQueryLocaties VerrijkMetVerrijkteAdressenUitGrar(VerrijkteAdressenUitGrar verrijkteAdressenUitGrar)
    {
        return new DuplicateVerenigingZoekQueryLocaties(
            _locaties.Select(x =>
            {
                if (x.AdresId is not null && verrijkteAdressenUitGrar.TryGetValue(x.AdresId.Bronwaarde, out var adres))
                {
                    return x with { Adres = adres };
                }

                return x;
            }).ToArray()
        );
    }

    public VerrijkteGemeentenaam[] VerrijkteGemeentes => Gemeentes.Select(x => VerrijkteGemeentenaam.FromGemeentenaam(new Gemeentenaam(x))).ToArray();

    public string[] Postcodes => _locaties.Select(l => l.Adres!.Postcode).ToArray();
    public string[] Gemeentes => _locaties.Select(l => l.Adres!.Gemeente.Naam).ToArray();
}
