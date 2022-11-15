namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Threading.Tasks;
using Vereniging;

public interface IVerenigingsRepository
{
    Task Save(Vereniging vereniging);
}
