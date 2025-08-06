namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Be.Vlaanderen.Basisregisters.AggregateSource;

public class LidmaatschapId : IntegerValueObject<LidmaatschapId>
{
    public static readonly LidmaatschapId InitialId = new(1);
    public LidmaatschapId Next
        => new LidmaatschapId(Value + 1);
    public LidmaatschapId(int id) : base(id)
    {
    }

    public static LidmaatschapId Max(LidmaatschapId id1, LidmaatschapId id2)
        => new(Math.Max(id1.Value, id2.Value));
}
