namespace AssociationRegistry.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;

public record MinimumScore(double Value)
{
    public static MinimumScore Default = new(3);
};
