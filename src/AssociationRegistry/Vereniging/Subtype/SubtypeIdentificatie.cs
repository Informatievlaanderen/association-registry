namespace AssociationRegistry.Vereniging;

using Framework;
using Be.Vlaanderen.Basisregisters.AggregateSource;

public class SubtypeIdentificatie: StringValueObject<SubtypeIdentificatie>
{
    private SubtypeIdentificatie(string @string) : base(@string)
    {
    }

    public static SubtypeIdentificatie Create(string beschrijving)
    {
        Throw<ArgumentNullException>.IfNull(beschrijving);

        return new(beschrijving);
    }

    public static SubtypeIdentificatie Hydrate(string beschrijving)
        => new(beschrijving);
}
