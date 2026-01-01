namespace AssociationRegistry.Framework.EventMetadata;

using Marten;

public record SourceFileMetadata(string SourceType, string? FileName) : IEventMetadata
{
    public static SourceFileMetadata KboSync(string? fileName)
        => new(SyncSources.KboSync, fileName);

    public static SourceFileMetadata KszSync(string? fileName)
        => new(SyncSources.KszSync, fileName);

    public void ApplyTo(IDocumentSession session)
    {
        session.SetHeader(MetadataHeaderNames.Source, SourceType);

        if (!string.IsNullOrEmpty(FileName))
            session.SetHeader(MetadataHeaderNames.SourceFileName, FileName);
    }
}
