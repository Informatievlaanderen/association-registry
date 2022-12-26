namespace AssociationRegistry.Vereniging;

using System.Threading.Tasks;
using Framework;

public interface IVerenigingsRepository
{
    Task<long> Save(Vereniging vereniging, CommandMetadata metadata);
}
