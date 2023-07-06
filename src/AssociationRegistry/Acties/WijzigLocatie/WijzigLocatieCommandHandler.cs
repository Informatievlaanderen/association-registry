namespace AssociationRegistry.Acties.WijzigLocatie;

using Framework;
using Vereniging;
using WijzigContactgegeven;

public class WijzigLocatieCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public WijzigLocatieCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigLocatieCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await _verenigingRepository.Load<Vereniging>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata.ExpectedVersion);

        var (locatieId, locatietype, isPrimair, naam, adres, adresId) = envelope.Command.TeWijzigenLocatie;
        vereniging.WijzigLocatie(locatieId, naam, locatietype, isPrimair, adresId, adres);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);
        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
