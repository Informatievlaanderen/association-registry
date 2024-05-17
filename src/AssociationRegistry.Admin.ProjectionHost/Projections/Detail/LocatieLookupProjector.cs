// namespace AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
//
// using Events;
// using Framework;
// using Infrastructure.Extensions;
// using Marten.Events;
// using Schema;
// using Schema.Detail;
// using IEvent = Marten.Events.IEvent;
//
// public class LocatieLookupProjector
// {
//     public static void Apply(IEvent<AdresWerdOvergenomenUitAdressenregister> @event, LocatieLookupDocument document)
//     {
//         document.Locaties = document.Locaties
//                                     .Where(loc => loc.LocatieId != @event.Data.LocatieId)
//                                     .Append(new LocatieLookupEntry
//                                      {
//                                          LocatieId = @event.Data.LocatieId,
//                                          AdresId = @event.Data.OvergenomenAdresUitAdressenregister.AdresId.Bronwaarde.Split('/').Last(),
//                                      })
//                                     .ToArray();
//     }
//
//     public static void UpdateMetadata(IEvent @event, LocatieLookupDocument document)
//     {
//         document.DatumLaatsteAanpassing = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
//         document.Metadata = new Metadata(@event.Sequence, @event.Version);
//     }
// }
