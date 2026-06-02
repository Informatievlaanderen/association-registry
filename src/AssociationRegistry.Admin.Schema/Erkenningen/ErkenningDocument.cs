namespace AssociationRegistry.Admin.Schema.Erkenningen;

using Marten.Schema;

public class ErkenningDocument
{
    [Identity] public string Id { get; set; } = null!;
    public string VCode { get; set; } = null!;
    public int ErkenningId { get; set; }
    public string? Status { get; set; }
    public DateOnly? Startdatum { get; set; }
    public DateOnly? Einddatum { get; set; }
}
