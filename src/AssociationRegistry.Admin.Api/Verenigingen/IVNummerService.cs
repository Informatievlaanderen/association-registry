namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Threading.Tasks;

public interface IVNummerService
{
    Task<string> GetNext();
}
