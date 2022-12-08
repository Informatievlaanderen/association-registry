namespace AssociationRegistry.Vereniging;

using System.Threading.Tasks;
using Framework;

public interface IVerenigingsRepository
{
    Task Save(Vereniging vereniging, CommandMetadata metadata);
}
