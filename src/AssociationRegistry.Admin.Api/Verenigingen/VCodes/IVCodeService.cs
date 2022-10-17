namespace AssociationRegistry.Admin.Api.Verenigingen.VCodes;

using System.Threading.Tasks;

public interface IVCodeService
{
    Task<VCode> GetNext();
}
