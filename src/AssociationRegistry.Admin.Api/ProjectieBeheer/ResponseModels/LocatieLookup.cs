namespace AssociationRegistry.Admin.Api.ProjectieBeheer.ResponseModels;

using System.Collections.Generic;


public record LocatiesMetAdresIdVolgensVCode
{
    public record LocatieLookup(int LocatieId, string AdresId);

    public string VCode { get; set; }
    public IEnumerable<LocatieLookup> Data { get; set; }
}

public record LocatiesMetAdresIdVolgensAdresId
{
    public record LocatieLookup(int LocatieId, string VCode);

    public string AdresId { get; set; }
    public IEnumerable<LocatieLookup> Data { get; set; }
}

