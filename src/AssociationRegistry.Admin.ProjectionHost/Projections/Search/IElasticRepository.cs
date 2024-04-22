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
    Task<VerenigingZoekDocument.Locatie> GetLocatie(string id, int locatieId);
    Task AppendLocatie<TDocument>(string id, ILocatie locatie) where TDocument : class;
    Task RemoveLocatie<TDocument>(string id, int locatieId) where TDocument : class;
    Task UpdateLocatie<TDocument>(string id, ILocatie locatie) where TDocument : class;
}
