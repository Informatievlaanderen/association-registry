namespace AssociationRegistry.Admin.Schema.Vertegenwoordiger;

using Marten.Schema;

public record VertegenwoordigersPerVCodeDocument()
{
    [property: Identity] public string VCode { get; set; }
    public VertegenwoordigerData[] VertegenwoordigersData { get; set; }
}

public record VertegenwoordigerData(int VertegenwoordigerId, string Status);

public static class VertegenwoordigerKszStatus
{
    public const string Created = "Created";
    public const string Bevestigd = "Bevestigd";
    public const string Overleden = "Overleden";
    public const string NietGekend = "NietGekend";
};
