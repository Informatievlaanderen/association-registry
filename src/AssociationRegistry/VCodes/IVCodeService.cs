namespace AssociationRegistry.VCodes;

using System.Threading.Tasks;

public interface IVCodeService
{
    Task<VCode> GetNext();
}
