namespace AssociationRegistry.Events;

public record KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend(int VertegenwoordigerId, string Insz, string Voornaam, string Achternaam) : IEvent;
public record KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendZonderPersoonsgegevens(Guid RefId, int VertegenwoordigerId) : IEvent;
