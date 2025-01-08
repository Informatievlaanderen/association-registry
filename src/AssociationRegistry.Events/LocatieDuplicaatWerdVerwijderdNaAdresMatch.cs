namespace AssociationRegistry.Events;



public record LocatieDuplicaatWerdVerwijderdNaAdresMatch(string VCode, int VerwijderdeLocatieId, int BehoudenLocatieId, string LocatieNaam, Registratiedata.AdresId AdresId) : IEvent;
