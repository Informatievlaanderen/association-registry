namespace AssociationRegistry.Admin.Api.Projections.Historiek.Schema;

public record BeheerVerenigingHistoriekGebeurtenis(
    string Beschrijving,
    string Gebeurtenis,
    object? Data = null,
    string Initiator = "",
    string Tijdstip = ""
);
