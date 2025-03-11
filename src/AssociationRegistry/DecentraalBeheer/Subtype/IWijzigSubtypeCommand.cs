namespace AssociationRegistry.DecentraalBeheer.Subtype;

using Vereniging;

public interface IWijzigSubtypeCommand
{
    VCode VCode { get; init; }
}
