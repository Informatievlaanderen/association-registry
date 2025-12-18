namespace AssociationRegistry.MartenDb.VertegenwoordigerPersoonsgegevens;

using DecentraalBeheer.Vereniging;

public record VertegenwoordigerPersoonsgegevensByInszFilter
{
    public Insz Insz { get; }

    public VertegenwoordigerPersoonsgegevensByInszFilter(Insz insz)
    {
        Insz = insz;
    }
}
