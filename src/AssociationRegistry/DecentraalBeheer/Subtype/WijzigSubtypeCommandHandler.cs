namespace AssociationRegistry.DecentraalBeheer.Subtype;

using Framework;
using Vereniging;

public class WijzigSubtypeCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public WijzigSubtypeCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<WijzigSubtypeCommand> envelope, CancellationToken cancellationToken = default)
    {
        var vereniging =
            await _verenigingRepository.Load<Vereniging>(
                VCode.Create(envelope.Command.VCode),
                envelope.Metadata.ExpectedVersion);

        switch (envelope.Command.SubtypeData)
        {
            case WijzigSubtypeCommand.TerugTeZettenNaarNogNietBepaald terugTeZettenNaarNogNietBepaald:
                vereniging.ZetSubtypeNaarNogNietBepaald();
                break;

            case WijzigSubtypeCommand.TeWijzigenNaarFeitelijkeVereniging teWijzigenNaarFeitelijkeVereniging:
                vereniging.WijzigSubtypeNaarFeitelijkeVereniging();
                break;

            case WijzigSubtypeCommand.TeWijzigenSubtype teWijzigenSubtype:
                //if (await _verenigingRepository.IsVerwijderd(teWijzigenSubtype.AndereVereniging))
                //    throw new VerenigingKanGeenLidWordenVanVerwijderdeVereniging();
                // vereniging.WijzigSubtype
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }


        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
