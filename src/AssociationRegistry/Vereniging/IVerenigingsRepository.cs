namespace AssociationRegistry.Vereniging;

using System.Threading.Tasks;
using EventStore;
using Framework;
using VCodes;

public interface IVerenigingsRepository
{
    Task<StreamActionResult> Save(Vereniging vereniging, CommandMetadata metadata);
    Task<Vereniging> Load(VCode vCode, long? expectedVersion);
}
