namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using ResultNet;

public record DuplicateKboFound(VCode VCode)
{
    public static Result WithVcode(VCode vCode) => new Result<DuplicateKboFound>(new DuplicateKboFound(vCode), ResultStatus.Failed);
}
