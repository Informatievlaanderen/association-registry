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
    public record LocatieLookup(string VCode, int LocatieId);

    public string AdresId { get; set; }
    public IEnumerable<LocatieLookup> Data { get; set; }
}

public record LocatiesMetAdresIdVolgensLocatieId
{
    public record LocatieLookup(string VCode, string AdresId);

    public int LocatieId { get; set; }
    public IEnumerable<LocatieLookup> Data { get; set; }
}
