namespace AssociationRegistry.Admin.Schema.Historiek;

public record BeheerVerenigingHistoriekGebeurtenis(
    string Beschrijving,
    string Gebeurtenis,
    object? Data = null,
    string Initiator = "",
    string Tijdstip = ""
);
