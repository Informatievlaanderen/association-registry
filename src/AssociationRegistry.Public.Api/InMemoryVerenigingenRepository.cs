// namespace AssociationRegistry.Public.Api;
//
// using System;
// using System.Collections.Immutable;
// using System.Linq;
// using System.Threading.Tasks;
// using DetailVerenigingen;
// using ListVerenigingen;
// using Activiteit = ListVerenigingen.Activiteit;
// using Locatie = ListVerenigingen.Locatie;
//
// public class InMemoryVerenigingenRepository : IVerenigingenRepository
// {
//     private readonly VerenigingListItem[] _verenigingenListItems;
//     private readonly VerenigingDetail[] _verenigingenDetails;
//
//     private InMemoryVerenigingenRepository(
//         VerenigingListItem[] verenigingenListItems,
//         VerenigingDetail[] verenigingenDetails)
//     {
//         _verenigingenListItems = verenigingenListItems;
//         _verenigingenDetails = verenigingenDetails;
//     }
//
//     public Task<ImmutableArray<VerenigingListItem>> List() =>
//         Task.FromResult(_verenigingenListItems.ToImmutableArray());
//
//     public Task<VerenigingDetail?> Detail(string vCode) =>
//         Task.FromResult(_verenigingenDetails.FirstOrDefault(v =>
//             string.Equals(v.Id, vCode, StringComparison.OrdinalIgnoreCase)));
//
//     public Task<int> TotalCount() => Task.FromResult(_verenigingenListItems.Length);
//
//     public static InMemoryVerenigingenRepository Create(string? baseUrl)
//         => new(
//             new VerenigingListItem[]
//             {
//                 new(
//                     "V1234567",
//                     "FWA De vrolijke BA’s",
//                     "DVB",
//                     new Locatie("1770", "Liedekerke"),
//                     ImmutableArray.Create(
//                         new Activiteit("Badminton", new Uri($"{baseUrl}v1/verenigingen/V000010")),
//                         new Activiteit("Tennis", new Uri($"{baseUrl}v1/verenigingen/V000010")))),
//             },
//             new VerenigingDetail[]
//             {
//                 new(
//                     "V1234567",
//                     "FWA De vrolijke BA’s",
//                     "DVB",
//                     "Korte omschrijving voor FWA De vrolijke BA's",
//                     "Feitelijke vereniging",
//                     DateOnly.FromDateTime(new DateTime(2022, 09, 15)),
//                     DateOnly.FromDateTime(new DateTime(2023, 10, 16)),
//                     new DetailVerenigingen.Locatie("1770", "Liedekerke"),
//                     new ContactPersoon(
//                         "Walter",
//                         "Plop",
//                         ImmutableArray.Create(
//                             new ContactGegeven("email", "walter.plop@studio100.be"),
//                             new ContactGegeven("telefoon", "100"))),
//                     ImmutableArray.Create(new DetailVerenigingen.Locatie("1770", "Liedekerke")),
//                     ImmutableArray.Create(
//                         new DetailVerenigingen.Activiteit("Badminton", new Uri($"{baseUrl}v1/verenigingen/V000010")),
//                         new DetailVerenigingen.Activiteit("Tennis", new Uri($"{baseUrl}v1/verenigingen/V000010"))),
//                     ImmutableArray.Create(
//                         new ContactGegeven("telefoon", "025462323"),
//                         new ContactGegeven("email", "info@dotimeforyou.be"),
//                         new ContactGegeven("website", "www.dotimeforyou.be"),
//                         new ContactGegeven("socialmedia", "facebook/dotimeforyou"),
//                         new ContactGegeven("socialmedia", "twitter/@dotimeforyou")
//                     ),
//                     DateOnly.FromDateTime(new DateTime(2022, 09, 26))),
//                 new(
//                     "V1234568",
//                     "FWA De vrolijke BA’s",
//                     String.Empty,
//                     String.Empty,
//                     String.Empty,
//                     null,
//                     null,
//                     new DetailVerenigingen.Locatie("1840", "Londerzeel"),
//                     null,
//                     ImmutableArray.Create(new DetailVerenigingen.Locatie("1840", "Londerzeel")),
//                     ImmutableArray<DetailVerenigingen.Activiteit>.Empty,
//                     ImmutableArray<ContactGegeven>.Empty,
//                     DateOnly.FromDateTime(new DateTime(2022, 09, 26))),
//             }
//         );
// }
