namespace AssociationRegistry.DecentraalBeheer.Acties.Subtype;

using Vereniging;

public interface IWijzigSubtypeCommand
{
    VCode VCode { get; init; }
}
