namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardDubbel;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record AanvaardDubbeleVerenigingCommand(VCode VCode, VCode VCodeDubbeleVereniging);
