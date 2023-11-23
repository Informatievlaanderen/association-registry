namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

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
    Task AppendLocatie<TDocument>(string id, ILocatie locatie) where TDocument : class;
    Task RemoveLocatie<TDocument>(string id, int locatieId) where TDocument : class;
    Task UpdateLocatie<TDocument>(string id, ILocatie locatie) where TDocument : class;
    Task Remove<T>(string id) where T : class;
}
