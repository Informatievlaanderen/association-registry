namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using Schema.Search;

public interface IElasticRepository
{
    void Index<TDocument>(TDocument document)
        where TDocument : class;

    Task IndexAsync<TDocument>(TDocument document)
        where TDocument : class;

    Task UpdateAsync<TDocument>(string id, TDocument update, long sequence) where TDocument : class;
    Task UpdateVerenigingsTypeAndClearSubverenigingVan<TDocument>(string id, string code, string naam, long sequence) where TDocument : class;
    Task UpdateStartdatum<TDocument>(string id, DateOnly? startdatum, long sequence) where TDocument : class;
    Task AppendLocatie<TDocument>(string id, ILocatie locatie, long sequence) where TDocument : class;
    Task AppendLidmaatschap<TDocument>(string id, ILidmaatschap lidmaatschap, long sequence) where TDocument : class;
    Task UpdateLidmaatschap<TDocument>(string id, ILidmaatschap lidmaatschap, long sequence) where TDocument : class;
    Task RemoveLidmaatschap<TDocument>(string id, int lidmaatschapId, long sequence) where TDocument : class;
    Task RemoveLocatie<TDocument>(string id, int locatieId, long sequence) where TDocument : class;
    Task UpdateLocatie<TDocument>(string id, ILocatie locatie, long sequence) where TDocument : class;
    Task UpdateAdres<TDocument>(string id, int locatieId, string adresVoorstelling, string postcode, string gemeente, long sequence) where TDocument : class;
    Task AppendCorresponderendeVCodes<TDocument>(string id, string vCodeDubbeleVereniging, long sequence) where TDocument : class;
    Task RemoveCorresponderendeVCode<TDocument>(string id, string vCodeDubbeleVereniging, long sequence) where TDocument : class;

    Task UpdateSubverenigingVanRelatie<TDocument>(
        string id,
        string andereVereniging,
        long sequence)
        where TDocument : class;
    Task UpdateSubverenigingVanDetail<TDocument>(
        string id,
        string identificatie,
        string beschrijving,
        long sequence)
        where TDocument : class;
}
