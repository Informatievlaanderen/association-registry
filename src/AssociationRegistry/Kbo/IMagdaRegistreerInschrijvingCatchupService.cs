namespace AssociationRegistry.Kbo;

public interface IMagdaRegistreerInschrijvingCatchupService
{
    Task RegistreerInschrijvingVoorVerenigingenMetRechtspersoonlijkheidDieNogNietIngeschrevenZijn();
    Task<IReadOnlyCollection<string>> GetKboNummersZonderRegistreerInschrijving();
}
