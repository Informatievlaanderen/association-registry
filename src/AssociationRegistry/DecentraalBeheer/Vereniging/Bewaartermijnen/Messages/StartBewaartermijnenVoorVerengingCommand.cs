namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;

using NodaTime;

public class StartBewaartermijnenVoorVerenigingCommand
{
    public string StreamKey { get; }
    public string PersoonsgegevensType { get; }
    public Instant Vervaldag { get; }
    public string Reden { get; }

    public StartBewaartermijnenVoorVerenigingCommand(
        string streamKey,
        string persoonsgegevensType,
        Instant vervaldag,
        string reden
    )
    {
        StreamKey = streamKey;
        Vervaldag = vervaldag;
        Reden = reden;
        PersoonsgegevensType = persoonsgegevensType;
    }
}
