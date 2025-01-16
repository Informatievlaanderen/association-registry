namespace AssociationRegistry.DuplicateVerenigingDetection;

public record MinimumScore(double Value)
{
    public static MinimumScore Default = new(1);
};
