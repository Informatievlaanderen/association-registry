namespace AssociationRegistry.Vereniging;

using VCodes;

public record CommandResult(VCode Vcode, long? Sequence, long? Version);
