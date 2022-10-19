namespace AssociationRegistry.Public.Api.Projections;

public interface IElasticRepository
{
    void Save<TDocument>(TDocument document)
        where TDocument : class;
}
