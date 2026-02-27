namespace AssociationRegistry.Admin.Schema.KboSync;

using Marten.Schema;

public record BeheerKszSyncHistoriekGebeurtenisDocument(
    [property: Identity] string Sequence,
    string VCode,
    int VertegenwooridgerId,
    string Beschrijving,
    string Tijdstip
);

