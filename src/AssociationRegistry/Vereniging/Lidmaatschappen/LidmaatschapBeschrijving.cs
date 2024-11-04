namespace AssociationRegistry.Vereniging;

using Framework;
using Be.Vlaanderen.Basisregisters.AggregateSource;

public class LidmaatschapBeschrijving: StringValueObject<LidmaatschapBeschrijving>
{
    private LidmaatschapBeschrijving(string @string) : base(@string)
    {
    }

    public static LidmaatschapBeschrijving Create(string beschrijving)
    {
        Throw<ArgumentNullException>.IfNull(beschrijving);

        return new(beschrijving);
    }

    public static LidmaatschapBeschrijving Hydrate(string beschrijving)
        => new(beschrijving);
}
