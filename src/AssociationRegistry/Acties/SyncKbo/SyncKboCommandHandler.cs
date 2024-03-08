namespace AssociationRegistry.Acties.SyncKbo;

using Framework;
using Kbo;
using Notifications;
using Notifications.Messages;
using ResultNet;
using Vereniging;
using Vereniging.Exceptions;

public class SyncKboCommandHandler
{
    private readonly IMagdaGeefVerenigingService _magdaGeefVerenigingService;
    private readonly INotifier _notifier;

    public SyncKboCommandHandler(
        IMagdaGeefVerenigingService magdaGeefVerenigingService,
        INotifier notifier)
    {
        _magdaGeefVerenigingService = magdaGeefVerenigingService;
        _notifier = notifier;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<SyncKboCommand> message,
        IVerenigingsRepository repository,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await repository.Load(message.Command.KboNummer, message.Metadata.ExpectedVersion);

        var verenigingVolgensMagda =
            await _magdaGeefVerenigingService.GeefVereniging(message.Command.KboNummer, message.Metadata, cancellationToken);

        if (verenigingVolgensMagda.IsFailure())
        {
            await _notifier.Notify(new KboSynchronisatieMisluktMessage(message.Command.KboNummer));
            throw new GeenGeldigeVerenigingInKbo();
        }

        vereniging.WijzigRechtsvormUitKbo(verenigingVolgensMagda.Data.Type);
        vereniging.WijzigNaamUitKbo(VerenigingsNaam.Create(verenigingVolgensMagda.Data.Naam ?? ""));
        vereniging.WijzigKorteNaamUitKbo(verenigingVolgensMagda.Data.KorteNaam ?? "");
        vereniging.WijzigStartdatum(Datum.CreateOptional(verenigingVolgensMagda.Data.Startdatum ?? null));
        vereniging.WijzigMaatschappelijkeZetelUitKbo(verenigingVolgensMagda.Data.Adres);
        HandleContactgegevens(vereniging, verenigingVolgensMagda);

        if (!verenigingVolgensMagda.Data.IsActief)
            vereniging.StopUitKbo(Datum.Create(verenigingVolgensMagda.Data.EindDatum!.Value));

        vereniging.SyncCompleted();

        var result = await repository.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(vereniging.VCode), result);
    }

    private static void HandleContactgegevens(
        VerenigingMetRechtspersoonlijkheid vereniging,
        Result<VerenigingVolgensKbo> verenigingVolgensMagda)
    {
        vereniging.WijzigContactgegevenUitKbo(verenigingVolgensMagda.Data.Contactgegevens.Email, ContactgegeventypeVolgensKbo.Email);
        vereniging.WijzigContactgegevenUitKbo(verenigingVolgensMagda.Data.Contactgegevens.Website, ContactgegeventypeVolgensKbo.Website);

        vereniging.WijzigContactgegevenUitKbo(verenigingVolgensMagda.Data.Contactgegevens.Telefoonnummer,
                                              ContactgegeventypeVolgensKbo.Telefoon);

        vereniging.WijzigContactgegevenUitKbo(verenigingVolgensMagda.Data.Contactgegevens.GSM, ContactgegeventypeVolgensKbo.GSM);
    }
}
