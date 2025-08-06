namespace AssociationRegistry.DecentraalBeheer.Vereniging.Subtypes.Subvereniging;

using Framework;
using Be.Vlaanderen.Basisregisters.AggregateSource;

public class SubverenigingBeschrijving: StringValueObject<SubverenigingBeschrijving>
{
    private SubverenigingBeschrijving(string @string) : base(@string)
    {
    }

    public static SubverenigingBeschrijving Create(string beschrijving)
    {
        Throw<ArgumentNullException>.IfNull(beschrijving);

        return new(beschrijving);
    }

    public static SubverenigingBeschrijving Hydrate(string beschrijving)
        => new(beschrijving);
}
