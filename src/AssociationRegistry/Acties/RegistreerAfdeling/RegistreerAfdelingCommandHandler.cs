namespace AssociationRegistry.Acties.RegistreerAfdeling;

using Framework;
using Vereniging;
using ResultNet;

public class RegistreerAfdelingCommandHandler
{
    private readonly IClock _clock;
    private readonly IVCodeService _vCodeService;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public RegistreerAfdelingCommandHandler(
        IVerenigingsRepository verenigingsRepository,
        IVCodeService vCodeService,
        IClock clock)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
        _clock = clock;
    }

    public async Task<Result> Handle(CommandEnvelope<RegistreerAfdelingCommand> message, CancellationToken cancellationToken = default)
    {
        var command = message.Command;

        var vCode = await _vCodeService.GetNext();

        var vereniging = Vereniging.RegistreerAfdeling(
            vCode,
            command.Naam,
            command.KboNummerMoedervereniging,
            command.KorteNaam,
            command.KorteBeschrijving,
            command.Startdatum,
            command.Contactgegevens,
            command.Locaties,
            message.Command.Vertegenwoordigers,
            command.HoofdactiviteitenVerenigingsloket,
            _clock);

        var result = await _verenigingsRepository.Save(vereniging, message.Metadata, cancellationToken);
        return Result.Success(CommandResult.Create(vCode, result));
    }
}
