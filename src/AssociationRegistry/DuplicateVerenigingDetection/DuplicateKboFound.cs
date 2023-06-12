namespace AssociationRegistry.DuplicateVerenigingDetection;

using ResultNet;
using Vereniging;

public record DuplicateKboFound(VCode VCode)
{
    public static Result WithVcode(VCode vCode)=>new Result<DuplicateKboFound>(new DuplicateKboFound(vCode), ResultStatus.Failed);
}
