namespace AssociationRegistry.Admin.Schema.PowerBiExport;

public record Bankrekeningnummer(
    int BankrekeningnummerId,
    string Iban,
    string GebruiktVoor,
    string Titularis);
