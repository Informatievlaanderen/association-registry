namespace AssociationRegistry.Admin.Api.Infrastructure.CommandMiddleware;

using AssociationRegistry.Framework;

public interface ICommandMetadataProvider
{
    CommandMetadata GetMetadata(long? expectedVersion = null);
}
