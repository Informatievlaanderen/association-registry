namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using Schema.Search;

public interface IElasticRepository
{
    Task IndexAsync<TDocument>(TDocument document)
        where TDocument : class;

    Task UpdateAsync<TDocument>(string id, TDocument update, long sequence) where TDocument : class;
    Task UpdateVerenigingsTypeAndClearSubverenigingVan<TDocument>(string id, string code, string naam, long sequence) where TDocument : class;
    Task AppendLocatie(string id, VerenigingZoekDocument.Types.Locatie locatie, long sequence);
    Task RemoveLocatie(string id, int locatieId, long sequence);
    Task UpdateLocatie(string id, VerenigingZoekDocument.Types.Locatie locatie, long sequence);
    Task Remove(string id);
    Task UpdateAdres(string messageVCode, int dataLocatieId, string toAdresString, string adresPostcode, string adresGemeente, long sequence);
    Task AppendLidmaatschap(string id, VerenigingZoekDocument.Types.Lidmaatschap lidmaatschap, long sequence);
    Task UpdateLidmaatschap(string id, VerenigingZoekDocument.Types.Lidmaatschap lidmaatschap, long sequence);
    Task RemoveLidmaatschap(string id, int lidmaatschapId, long sequence);

    Task UpdateSubverenigingVanRelatie<TDocument>(
        string id,
        string andereVereniging,
        long sequence)where TDocument : class;

    Task UpdateSubverenigingVanDetail<TDocument>(
        string id,
        string identificatie,
        string beschrijving,
        long sequence)where TDocument : class;
}
