namespace AssociationRegistry.DecentraalBeheer.Subtype;

using Vereniging;

public interface IWijzigSubtypeCommand
{
    VCode VCode { get; init; }
}

public record ZetSubtypeTerugNaarNogNietBepaaldCommand(VCode VCode) : IWijzigSubtypeCommand
{ }
