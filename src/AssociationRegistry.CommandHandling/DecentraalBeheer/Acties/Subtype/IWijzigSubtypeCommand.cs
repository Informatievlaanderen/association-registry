namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Subtype;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public interface IWijzigSubtypeCommand
{
    VCode VCode { get; init; }
}
