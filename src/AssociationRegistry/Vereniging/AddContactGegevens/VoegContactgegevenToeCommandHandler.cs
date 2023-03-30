namespace AssociationRegistry.Vereniging.AddContactgegevens;

using ContactGegevens;
using Framework;
using VCodes;

public class VoegContactgegevenToeCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public VoegContactgegevenToeCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<VoegContactgegevenToeCommand> envelope)
    {
        var vereniging = await _verenigingRepository.Load(VCode.Create(envelope.Command.VCode), envelope.Metadata.ExpectedVersion);

        var contactgegeven = Contactgegeven.Create(envelope.Command.Contactgegeven.Type, envelope.Command.Contactgegeven.Waarde, envelope.Command.Contactgegeven.Omschrijving, envelope.Command.Contactgegeven.IsPrimair);
        vereniging.VoegContactgegevenToe(contactgegeven);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata);
        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
