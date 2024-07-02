namespace AssociationRegistry.Grar.AddressSync;

public class SynchroniseerLocatieMessageHandler()
{
    public async Task Handle(SynchroniseerLocatieMessage message)
    {
        // var vereniging = await repository.Load<VerenigingOfAnyKind>(VCode.Create(resultByVCode.VCode));
        //
        // try
        // {
        //     foreach (var locatieWithAdres in resultByVCode.LocatieWithAdres)
        //         await vereniging.HeradresseerLocatie(locatieWithAdres, idempotenceKey, grarClient);
        //
        //     await repository.Save(
        //         vereniging,
        //         new CommandMetadata(EventStore.DigitaalVlaanderenOvoNumber,
        //                             SystemClock.Instance.GetCurrentInstant(),
        //                             Guid.NewGuid()),
        //         cancellationToken);
        // }
        // catch (Exception ex)
        // {
        //     logger.LogError(ex, "Fout bij het synchroniseren van vereniging {VCode} voor locatie {LocatieId}", vereniging.VCode);
        // }
    }
}
