namespace AssociationRegistry.Test.Admin.Api.DuplicateDetection.Given_An_Extensive_DataSet;

using CsvHelper.Configuration.Attributes;

public record DuplicateDetectionSeedLine(
    [property: Name("Naam")] string Naam,
    [property: Name("TeRegistrerenNaam")] string TeRegistrerenNaam);
