namespace AssociationRegistry.Vereniging;

using Framework;
using Be.Vlaanderen.Basisregisters.AggregateSource;

public class SubtypeBeschrijving: StringValueObject<SubtypeBeschrijving>
{
    private SubtypeBeschrijving(string @string) : base(@string)
    {
    }

    public static SubtypeBeschrijving Create(string beschrijving)
    {
        Throw<ArgumentNullException>.IfNull(beschrijving);

        return new(beschrijving);
    }

    public static SubtypeBeschrijving Hydrate(string beschrijving)
        => new(beschrijving);
}
