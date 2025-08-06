namespace AssociationRegistry.DecentraalBeheer.Vereniging;

public interface IVCodeService
{
    Task<VCode> GetNext();
}
