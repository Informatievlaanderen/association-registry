namespace AssociationRegistry.Admin.Api.Infrastructure.Middleware;

using Framework;

public interface ICommandMetadataProvider
{
    CommandMetadata GetMetadata(long? expectedVersion = null);
}
