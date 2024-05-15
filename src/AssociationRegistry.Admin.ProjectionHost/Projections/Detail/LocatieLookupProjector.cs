namespace AssociationRegistry.Admin.ProjectionHost.Projections.Detail;

using Events;
using Framework;
using Infrastructure.Extensions;
using Marten.Events;
using Schema;
using Schema.Detail;
using IEvent = Marten.Events.IEvent;

public class LocatieLookupProjector
{
   public static void Apply(
        IEvent<AdresWerdOvergenomenUitAdressenregister> adresWerdOvergenomenUitAdressenregister,
        LocatieLookupDocument document)
   {
       document.AdresId = adresWerdOvergenomenUitAdressenregister.Data.OvergenomenAdresUitAdressenregister.AdresId.Bronwaarde.Split('/').Last();
       document.LocatieId = adresWerdOvergenomenUitAdressenregister.Data.LocatieId;
    }

    public static void UpdateMetadata(IEvent e, LocatieLookupDocument document)
    {
        document.DatumLaatsteAanpassing = e.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
        document.Metadata = new Metadata(e.Sequence, e.Version);
    }
}
