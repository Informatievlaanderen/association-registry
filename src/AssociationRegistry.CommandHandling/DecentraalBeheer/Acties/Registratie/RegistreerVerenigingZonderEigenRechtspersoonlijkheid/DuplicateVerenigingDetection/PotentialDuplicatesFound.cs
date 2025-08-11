namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;

public record PotentialDuplicatesFound(params DuplicaatVereniging[] PotentialDuplicates)
{
    public static PotentialDuplicatesFound None => new();
    public static PotentialDuplicatesFound Some(params DuplicaatVereniging[] potentialDuplicates) => new(potentialDuplicates);

    public bool HasDuplicates => PotentialDuplicates.Length > 0;
}
