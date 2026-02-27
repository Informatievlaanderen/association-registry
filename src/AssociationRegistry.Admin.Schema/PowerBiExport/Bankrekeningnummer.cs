namespace AssociationRegistry.Admin.Schema.PowerBiExport;

public record Bankrekeningnummer(
    int BankrekeningnummerId,
    string Doel,
    string[] BevestigdDoor,
    string Bron);
