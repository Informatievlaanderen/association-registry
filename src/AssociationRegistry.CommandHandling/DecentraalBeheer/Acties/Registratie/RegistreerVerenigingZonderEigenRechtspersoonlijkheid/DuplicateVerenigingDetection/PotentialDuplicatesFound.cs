namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;

using AssociationRegistry.DecentraalBeheer.Vereniging.DubbelDetectie;

public record PotentialDuplicatesFound(bool PotentialDuplicatesSkipped, string Bevestigingstoken, params DuplicaatVereniging[] PotentialDuplicates)
{
    public static PotentialDuplicatesFound None => new(false, string.Empty);
    public static PotentialDuplicatesFound Skip(string bevestigingstoken) => new(true, bevestigingstoken);
    public static PotentialDuplicatesFound Some(params DuplicaatVereniging[] potentialDuplicates) => new(false, string.Empty, potentialDuplicates);

    public bool HasDuplicates => PotentialDuplicates.Length > 0;
}
