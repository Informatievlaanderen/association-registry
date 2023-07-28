namespace AssociationRegistry.Acties.RegistreerVerenigingUitKbo;

using DuplicateVerenigingDetection;
using Framework;
using Kbo;
using Vereniging;
using ResultNet;
using Vereniging.Exceptions;

public class RegistreerVerenigingUitKboCommandHandler
{
    private readonly IVCodeService _vCodeService;
    private readonly IMagdaGeefVerenigingService _magdaGeefVerenigingService;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public RegistreerVerenigingUitKboCommandHandler(
        IVerenigingsRepository verenigingsRepository,
        IVCodeService vCodeService,
        IMagdaGeefVerenigingService magdaGeefVerenigingService)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
        _magdaGeefVerenigingService = magdaGeefVerenigingService;
    }

    public async Task<Result> Handle(CommandEnvelope<RegistreerVerenigingUitKboCommand> message, CancellationToken cancellationToken = default)
    {
        var command = message.Command;

        var duplicateResult = await CheckForDuplicate(command.KboNummer);
        if (duplicateResult.IsFailure()) return duplicateResult;

        var vereniging = await _magdaGeefVerenigingService.GeefVereniging(command.KboNummer, message.Metadata.Initiator, cancellationToken);
        if (vereniging.IsFailure()) throw new GeenGeldigeVerenigingInKbo();
        ValidateVereniging(vereniging.Data);

        return Result.Success(await RegisteerVereniging(vereniging, message.Metadata, cancellationToken));
    }

    private static void ValidateVereniging(VerenigingVolgensKbo verenigingVolgensKbo)
    {
        Throw<GeenGeldigeVerenigingInKbo>.If(verenigingVolgensKbo.Rechtsvorm is not "VZW" or "iVZW" or "Private Stichting" or "Stichting van openbaar nut");
    }

    private async Task<Result> CheckForDuplicate(KboNummer kboNummer)
    {
        var duplicateKbo = await _verenigingsRepository.GetVCodeAndNaam(kboNummer);
        return duplicateKbo is not null ? DuplicateKboFound.WithVcode(duplicateKbo.VCode!) : Result.Success();
    }

    private async Task<CommandResult> RegisteerVereniging(VerenigingVolgensKbo verenigingVolgensKbo, CommandMetadata messageMetadata, CancellationToken cancellationToken)
    {
        var vCode = await _vCodeService.GetNext();

        var vereniging = VerenigingMetRechtspersoonlijkheid.Registreer(
            vCode,
            verenigingVolgensKbo);

        var result = await _verenigingsRepository.Save(vereniging, messageMetadata, cancellationToken);
        return CommandResult.Create(vCode, result);
    }
}
