namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using System.Threading.Tasks;
using Schema.Search;

public interface IElasticRepository
{
    void Index<TDocument>(TDocument document)
        where TDocument : class;

    Task IndexAsync<TDocument>(TDocument document)
        where TDocument : class;

    void Update<TDocument>(string id, TDocument update) where TDocument : class;
    Task UpdateAsync<TDocument>(string id, TDocument update) where TDocument : class;
    Task AppendLocatie(string id, VerenigingZoekDocument.Locatie locatie);
    Task RemoveLocatie(string id, int locatieId);
    Task ReplaceLocatie(string id, VerenigingZoekDocument.Locatie locatie);
}
