namespace AssociationRegistry.Magda.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using ResultNet;

public interface IMagdaRegistreerInschrijvingService
{
    Task<Result> RegistreerInschrijving(
        KboNummer kboNummer,
        AanroependeFunctie aanroependeFunctie,
        CommandMetadata metadata,
        CancellationToken cancellationToken);
}
public record AanroependeDienst(string Naam)
{
    public static AanroependeDienst BeheerApi = new("Verenigingsregister Beheer Api");
}
public record AanroependeFunctie(AanroependeDienst AanroependeDienst, string Naam)
{
    public static readonly AanroependeFunctie RegistreerVzer = new(AanroependeDienst.BeheerApi, "Registreer inschrijving voor vereniging zonder rechtspersoonlijkheid");
    public static readonly AanroependeFunctie RegistreerVerenigingMetRechtspersoonlijkheid = new(AanroependeDienst.BeheerApi, "Registreer vereniging met rechtspersoonlijkheid");
    public static readonly AanroependeFunctie SyncKbo = new(AanroependeDienst.BeheerApi, "Sync vereniging met rechtspersoonlijkheid");
}
