namespace AssociationRegistry.Vereniging;

public interface IVCodeService
{
    Task<VCode> GetNext();
}
