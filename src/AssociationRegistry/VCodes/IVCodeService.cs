namespace AssociationRegistry.VCodes;

public interface IVCodeService
{
    Task<VCode> GetNext();
}
