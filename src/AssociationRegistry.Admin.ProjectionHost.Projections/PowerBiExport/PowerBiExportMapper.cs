namespace AssociationRegistry.Admin.ProjectionHost.Projections.PowerBiExport;

using Contracts.JsonLdContext;
using Schema;

public class PowerBiExportMapper
{


    public static Schema.PowerBiExport.Bankrekeningnummer MapBankrekeningnummer(
        int bankrekeningnummerId,
        string doel,
        string[] bevestigdDoor,
        string bron
    ) =>
        new(bankrekeningnummerId, doel, bevestigdDoor, bron);

    public static JsonLdMetadata CreateJsonLdMetadata(JsonLdType jsonLdType, params string[] idValues) =>
        new() { Id = jsonLdType.CreateWithIdValues(idValues), Type = jsonLdType.Type };
}
