namespace AssociationRegistry.Public.Api.Projections.Search;

using System.Threading.Tasks;

public interface IElasticRepository
{
    void Index<TDocument>(TDocument document)
        where TDocument : class;

    Task IndexAsync<TDocument>(TDocument document)
        where TDocument : class;
}
