namespace AssociationRegistry.Events;

public record KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden(int VertegenwoordigerId, string Insz, string Voornaam, string Achternaam) : IEvent;
public record KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenZonderPersoonsgegevens(Guid RefId, int VertegenwoordigerId) : IEvent;
