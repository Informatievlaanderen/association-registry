namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Threading.Tasks;

public interface IVCodeService
{
    Task<string> GetNext();
}
