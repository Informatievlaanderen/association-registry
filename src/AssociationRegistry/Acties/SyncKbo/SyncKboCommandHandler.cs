namespace AssociationRegistry.Acties.SyncKbo;

using Framework;
using Kbo;
using ResultNet;
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
        vereniging.WijzigStartdatum(Datum.CreateOptional(verengigingVolgensMagda.Data.Startdatum));
        HandleContactgegevens(vereniging, verengigingVolgensMagda);

        vereniging.SyncCompleted();

        var result = await repository.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(vereniging.VCode), result);
    }

    private static void HandleContactgegevens(VerenigingMetRechtspersoonlijkheid vereniging, Result<VerenigingVolgensKbo> verenigingVolgensMagda)
    {
        vereniging.WijzigContactgegevenUitKbo(verenigingVolgensMagda.Data.Contactgegevens.Email, ContactgegeventypeVolgensKbo.Email);
        vereniging.WijzigContactgegevenUitKbo(verenigingVolgensMagda.Data.Contactgegevens.Website, ContactgegeventypeVolgensKbo.Website);
        vereniging.WijzigContactgegevenUitKbo(verenigingVolgensMagda.Data.Contactgegevens.Telefoonnummer, ContactgegeventypeVolgensKbo.Telefoon);
        vereniging.WijzigContactgegevenUitKbo(verenigingVolgensMagda.Data.Contactgegevens.GSM, ContactgegeventypeVolgensKbo.GSM);
    }
}
