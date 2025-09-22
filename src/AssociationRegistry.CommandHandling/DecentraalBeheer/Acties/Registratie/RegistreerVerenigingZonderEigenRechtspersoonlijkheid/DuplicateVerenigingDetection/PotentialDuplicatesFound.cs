namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;

public record PotentialDuplicatesFound(bool PotentialDuplicatesSkipped, params DuplicaatVereniging[] PotentialDuplicates)
{
    public static PotentialDuplicatesFound None => new(false);
    public static PotentialDuplicatesFound Skip => new(true);
    public static PotentialDuplicatesFound Some(params DuplicaatVereniging[] potentialDuplicates) => new(false, potentialDuplicates);

    public bool HasDuplicates => PotentialDuplicates.Length > 0;
}
