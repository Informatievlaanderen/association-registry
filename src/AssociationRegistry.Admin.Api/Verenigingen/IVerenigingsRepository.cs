namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Threading.Tasks;

public interface IVerenigingsRepository
{
    Task Save(Vereniging vereniging);
}
