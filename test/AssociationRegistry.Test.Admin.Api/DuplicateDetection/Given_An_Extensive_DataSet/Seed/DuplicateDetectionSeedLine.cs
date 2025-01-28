namespace AssociationRegistry.Test.Admin.Api.DuplicateDetection.Given_An_Extensive_DataSet.Seed;

using CsvHelper.Configuration.Attributes;

public record DuplicateDetectionSeedLine(
    [property: Name("GeregistreerdeNaam")] string GeregistreerdeNaam,
    [property: Name("TeRegistrerenDubbel")] string TeRegistrerenDubbel,
    [property: Name("TeRegistrerenGeenDubbel")] string TeRegistrerenGeenDubbel)
{
    public ICollection<string> TeRegistrerenDubbelNamen => TeRegistrerenDubbel.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
    public ICollection<string> TeRegistrerenGeenDubbelNamen => TeRegistrerenGeenDubbel.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
};
