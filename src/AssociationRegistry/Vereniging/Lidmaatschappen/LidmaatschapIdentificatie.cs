namespace AssociationRegistry.Vereniging;

using Framework;
using Be.Vlaanderen.Basisregisters.AggregateSource;

public class LidmaatschapIdentificatie: StringValueObject<LidmaatschapIdentificatie>
{
    private LidmaatschapIdentificatie(string @string) : base(@string)
    {
    }

    public static LidmaatschapIdentificatie Create(string beschrijving)
    {
        Throw<ArgumentNullException>.IfNull(beschrijving);

        return new(beschrijving);
    }

    public static LidmaatschapIdentificatie Hydrate(string beschrijving)
        => new(beschrijving);
}
