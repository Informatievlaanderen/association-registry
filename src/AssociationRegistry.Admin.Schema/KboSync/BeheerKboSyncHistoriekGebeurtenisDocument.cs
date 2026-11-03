namespace AssociationRegistry.Admin.Schema.KboSync;

using JasperFx;
using Marten.Schema;

public record BeheerKboSyncHistoriekGebeurtenisDocument(
    [property: Identity] string Sequence,
    string Kbonummer,
    string VCode,
    string Beschrijving,
    string Tijdstip
);
