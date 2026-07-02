namespace AssociationRegistry.Admin.Schema.PowerBiExport;

public record Bankrekeningnummer
{
    public int BankrekeningnummerId { get; set; }
    public string Doel { get; set; }
    public GegevensInitiator[] BevestigdDoor { get; set; }
    public string Bron { get; set; }

    public Bankrekeningnummer(int bankrekeningnummerId, string doel, GegevensInitiator[] bevestigdDoor, string bron)
    {
        BankrekeningnummerId = bankrekeningnummerId;
        Doel = doel;
        BevestigdDoor = bevestigdDoor;
        Bron = bron;
    }
}
