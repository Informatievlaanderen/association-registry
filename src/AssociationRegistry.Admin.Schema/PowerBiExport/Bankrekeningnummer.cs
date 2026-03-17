namespace AssociationRegistry.Admin.Schema.PowerBiExport;

public record Bankrekeningnummer
{
    public int BankrekeningnummerId { get; set; }
    public string Doel { get; set; }
    public string[] BevestigdDoor { get; set; }
    public string Bron { get; set; }

    public Bankrekeningnummer(
        int bankrekeningnummerId,
        string doel,
        string[] bevestigdDoor,
        string bron)
    {
        BankrekeningnummerId = bankrekeningnummerId;
        Doel = doel;
        BevestigdDoor = bevestigdDoor;
        Bron = bron;
    }

    public static Bankrekeningnummer Create(
        int bankrekeningnummerId,
        string doel,
        string bron
        )
        => new(
            bankrekeningnummerId,
            doel,
            [],
            bron);
}
