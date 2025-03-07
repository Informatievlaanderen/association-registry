namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using Schema.Search;

public interface IElasticRepository
{
    void Index<TDocument>(TDocument document)
        where TDocument : class;

    Task IndexAsync<TDocument>(TDocument document)
        where TDocument : class;

    void Update<TDocument>(string id, TDocument update) where TDocument : class;
    Task UpdateAsync<TDocument>(string id, TDocument update) where TDocument : class;
    Task UpdateStartdatum<TDocument>(string id, DateOnly? startdatum) where TDocument : class;
    Task<VerenigingZoekDocument.Types.Locatie> GetLocatie(string id, int locatieId);
    Task AppendLocatie<TDocument>(string id, ILocatie locatie) where TDocument : class;
    Task AppendLidmaatschap<TDocument>(string id, ILidmaatschap lidmaatschap) where TDocument : class;
    Task UpdateLidmaatschap<TDocument>(string id, ILidmaatschap lidmaatschap) where TDocument : class;
    Task RemoveLidmaatschap<TDocument>(string id, int lidmaatschapId) where TDocument : class;
    Task RemoveLocatie<TDocument>(string id, int locatieId) where TDocument : class;
    Task UpdateLocatie<TDocument>(string id, ILocatie locatie) where TDocument : class;
    Task UpdateAdres<TDocument>(string id, int locatieId, string adresVoorstelling, string postcode, string gemeente) where TDocument : class;
    Task AppendCorresponderendeVCodes<TDocument>(string id, string vCodeDubbeleVereniging) where TDocument : class;
    Task RemoveCorresponderendeVCode<TDocument>(string id, string vCodeDubbeleVereniging) where TDocument : class;
}
