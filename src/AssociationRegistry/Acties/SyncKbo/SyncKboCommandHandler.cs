namespace AssociationRegistry.Acties.SyncKbo;

using Framework;
using Kbo;
using Vereniging;

public class SyncKboCommandHandler
{
    private readonly IMagdaGeefVerenigingService _magdaGeefVerenigingService;

    public SyncKboCommandHandler(
        IMagdaGeefVerenigingService magdaGeefVerenigingService)
    {
        _magdaGeefVerenigingService = magdaGeefVerenigingService;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<SyncKboCommand> message,
        IVerenigingsRepository repository,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await repository.Load(message.Command.KboNummer, message.Metadata.ExpectedVersion);

        var verengigingVolgensMagda =
            await _magdaGeefVerenigingService.GeefVereniging(message.Command.KboNummer, message.Metadata, cancellationToken);

        vereniging.WijzigNaamUitKbo(VerenigingsNaam.Create(verengigingVolgensMagda.Data.Naam));
        vereniging.WijzigKorteNaamUitKbo(verengigingVolgensMagda.Data.KorteNaam);
        vereniging.WijzigContactgegevensUitKbo(verengigingVolgensMagda.Data.Contactgegevens);

        vereniging.SyncCompleted();

        var result = await repository.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(vereniging.VCode), result);
    }
}
