namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using Schema.Search;

public interface IElasticRepository
{
    void Index<TDocument>(TDocument document)
        where TDocument : class;

    Task IndexAsync<TDocument>(TDocument document)
        where TDocument : class;

    void Update<TDocument>(string id, TDocument update) where TDocument : class;
    Task UpdateAsync<TDocument>(string id, TDocument update) where TDocument : class;
    Task<VerenigingZoekDocument.Locatie> GetLocatie(string id, int locatieId);
    Task AppendLocatie(string id, VerenigingZoekDocument.Locatie locatie);
    Task RemoveLocatie(string id, int locatieId);
    Task UpdateLocatie(string id, VerenigingZoekDocument.Locatie locatie);
    Task Remove(string id);
    Task AppendRelatie(string id, Relatie relatie);
    Task UpdateAdres(string messageVCode, int dataLocatieId, string toAdresString, string adresPostcode, string adresGemeente);
    Task AppendLidmaatschap(string id, VerenigingZoekDocument.Lidmaatschap lidmaatschap);
    Task UpdateLidmaatschap(string id, VerenigingZoekDocument.Lidmaatschap lidmaatschap);
    Task RemoveLidmaatschap(string id, int lidmaatschapId);
}
