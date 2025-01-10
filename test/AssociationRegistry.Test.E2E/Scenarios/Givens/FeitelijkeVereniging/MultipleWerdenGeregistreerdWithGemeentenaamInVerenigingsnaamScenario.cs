namespace AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;

public class MultipleWerdenGeregistreerdWithGemeentenaamInVerenigingsnaamScenario : Framework.TestClasses.IScenario
{
    private CommandMetadata Metadata;

    public MultipleWerdenGeregistreerdWithGemeentenaamInVerenigingsnaamScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();



        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        var events = fixture.CreateMany<FeitelijkeVerenigingWerdGeregistreerd>(17).ToArray();

        events[0] = events[0] with { Naam = "KORTRIJK SPURS" };
        events[1] = events[1] with { Naam = "JUDOSCHOOL KORTRIJK" };
        events[2] = events[2] with { Naam = "Lebad Kortrijk" };
        events[3] = events[3] with { Naam = "Reddersclub Kortrijk" };
        events[4] = events[4] with { Naam = "Koninklijke Turnvereniging Kortrijk" };
        events[5] = events[5] with { Naam = "JONG KORTRIJK VOETBALT" };
        events[6] = events[6] with { Naam = "Kortrijkse Zwemkring" };
        events[7] = events[7] with { Naam = "KORTRIJKS SYMFONISCH ORKEST" };
        events[8] = events[8] with { Naam = "KONINKLIJK KORTRIJK SPORT ATLETIEK" };
        events[9] = events[9] with { Naam = "Kortrijkse Ultimate Frisbee Club" };
        events[10] = events[10] with { Naam = "Ruygi KORTRIJK" };
        events[11] = events[11] with { Naam = "Ruygo Judoschool KORTRIJK" };
        events[12] = events[12] with { Naam = "Schaakclub Kortrijk" };
        events[13] = events[13] with { Naam = "Wielerclub FC De ratjes" };
        events[14] = events[14] with { Naam = "Club Kortrijk" };
        events[15] = events[15] with { Naam = "Kortrijkse C# fanclub" };
        events[16] = events[16] with { Naam = "Clubben met de vrienden" };


        events = events.Select(
            @event => @event with
            {
                Locaties = [@event.Locaties.First() with { Adres = @event.Locaties.First().Adres with { Postcode = "8500" } }]
            }
        ).ToArray();

        return events.Select(x => new KeyValuePair<string, IEvent[]>(x.VCode, [x])).ToArray();
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
