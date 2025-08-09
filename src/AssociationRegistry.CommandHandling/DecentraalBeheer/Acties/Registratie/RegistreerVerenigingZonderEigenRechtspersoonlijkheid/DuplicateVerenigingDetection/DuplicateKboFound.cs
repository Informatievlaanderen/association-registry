namespace AssociationRegistry.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;

using Vereniging;
using ResultNet;

public record DuplicateKboFound(VCode VCode)
{
    public static Result WithVcode(VCode vCode) => new Result<DuplicateKboFound>(new DuplicateKboFound(vCode), ResultStatus.Failed);
}
