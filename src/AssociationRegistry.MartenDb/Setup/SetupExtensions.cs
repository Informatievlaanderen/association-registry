namespace AssociationRegistry.MartenDb.Setup;

using Marten;
using Microsoft.Extensions.DependencyInjection;
using Upcasters;

public static class SetupExtensions
{
    public static void UpcastLegacyTombstoneEvents(this StoreOptions source)
    {
        source.Events.Upcast(
            new TombstoneUpcaster()
        );
    }
}
