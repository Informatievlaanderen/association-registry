namespace AssociationRegistry.Vereniging;

using System.Threading.Tasks;

public interface IVerenigingsRepository
{
    Task Save(Vereniging vereniging);
}
