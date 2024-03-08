namespace AssociationRegistry.Kbo;

using Vereniging;

public interface IMagdaRegistreerInschrijvingCatchupService
{
    Task RegistreerInschrijvingVoorVerenigingenMetRechtspersoonlijkheidDieNogNietIngeschrevenZijn();
    Task<IReadOnlyCollection<KboNummer>> GetKboNummersZonderRegistreerInschrijving();
}
