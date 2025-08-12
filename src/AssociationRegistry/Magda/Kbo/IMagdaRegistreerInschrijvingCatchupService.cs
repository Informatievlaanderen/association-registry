namespace AssociationRegistry.Magda.Kbo;

public interface IMagdaRegistreerInschrijvingCatchupService
{
    Task RegistreerInschrijvingVoorVerenigingenMetRechtspersoonlijkheidDieNogNietIngeschrevenZijn();
    Task<IReadOnlyCollection<string>> GetKboNummersZonderRegistreerInschrijving();
}
