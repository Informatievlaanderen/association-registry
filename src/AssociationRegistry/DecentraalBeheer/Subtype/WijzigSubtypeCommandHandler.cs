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

            //if (await _verenigingRepository.IsVerwijderd(teWijzigenSubtype.AndereVereniging))
            //    throw new VerenigingKanGeenLidWordenVanVerwijderdeVereniging();
            // vereniging.WijzigSubtype


            // if state.subtype is subvereniging
            // -> wijzigSubtype
            // -> validate rond welke velden mogen (minstens 1 ingevuld)
            // if not empty pak beschrijfing & identificatie over

            // else
            // verfijn naar subverening
            // if not empty pak beschrijfing & identificatie over




        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
