namespace AssociationRegistry.Vereniging.Subtypes.Subvereniging;

using AssociationRegistry.Framework;
using Be.Vlaanderen.Basisregisters.AggregateSource;

public class SubverenigingIdentificatie: StringValueObject<SubverenigingIdentificatie>
{
    private SubverenigingIdentificatie(string @string) : base(@string)
    {
    }

    public static SubverenigingIdentificatie Create(string beschrijving)
    {
        Throw<ArgumentNullException>.IfNull(beschrijving);

        return new(beschrijving);
    }

    public static SubverenigingIdentificatie Hydrate(string beschrijving)
        => new(beschrijving);
}
