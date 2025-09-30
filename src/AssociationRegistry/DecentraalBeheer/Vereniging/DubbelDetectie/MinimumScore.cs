namespace AssociationRegistry.DecentraalBeheer.Vereniging.DubbelDetectie;

public record MinimumScore(double Value)
{
    public static MinimumScore Default = new(3);
};
