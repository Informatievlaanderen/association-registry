namespace AssociationRegistry.Vereniging;

using System.Threading.Tasks;
using Framework;
using VCodes;

public interface IVerenigingsRepository
{
    Task<long?> Save(Vereniging vereniging, CommandMetadata metadata);
    Task<Vereniging> Load(VCode vCode);
}
