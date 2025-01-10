namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging_With_Duplicates_With_Gemeentenaam_In_Verenigingsnaam;

using Admin.Api.DecentraalBeheer.Verenigingen.Common;
using Admin.Api.Infrastructure;
using Alba;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;
using AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.AutoFixture;
using Events;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Newtonsoft.Json.Linq;
using Scenarios.Requests;
using System.Net;

public class RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaamContext: TestContextBase<RegistreerFeitelijkeVerenigingRequest[]>
{
        public class TestData
    {
        public string KortrijkSpurs { get; set; } = $"{FictieveGemeentenaam.ToUpper()} SPURS";
        public string JudoschoolKortrijk { get; set; } = $"JUDOSCHOOL {FictieveGemeentenaam.ToUpper()}";
        public string LebadKortrijk { get; set; } = $"Lebad {FictieveGemeentenaam}";
        public string ReddersclubKortrijk { get; set; } = $"Reddersclub {FictieveGemeentenaam}";
        public string KoninklijkeTurnverenigingKortrijk { get; set; } = $"Koninklijke Turnvereniging {FictieveGemeentenaam}";
        public string JongKortrijkVoetbalt { get; set; } = $"JONG {FictieveGemeentenaam} VOETBALT";
        public string KortrijkseZwemkring { get; set; } = $"{FictieveGemeentenaam}se Zwemkring";
        public string KortrijksSymfonischOrkest { get; set; } = $"{FictieveGemeentenaam}se SYMFONISCH ORKEST";
        public string KoninklijkKortrijkSportAtletiek { get; set; } = $"KONINKLIJK {FictieveGemeentenaam} SPORT ATLETIEK";
        public string KortrijkseUltimateFrisbeeClub { get; set; } = $"{FictieveGemeentenaam}se Ultimate Frisbee Club";
        public string RuygiKortrijk { get; set; } = $"Ruygi {FictieveGemeentenaam}";
        public string RuygoJudoschoolKortrijk { get; set; } = $"Ruygo Judoschool {FictieveGemeentenaam}";

        public List<FeitelijkeVerenigingWerdGeregistreerd> Events { get; set; }
        public static string FictieveGemeentenaam = "FictieveGemeentenaam";
        public static string FictievePostcode = "AAAA";

        public TestData()
        {
            var fixture = new Fixture().CustomizeAdminApi();

            Events = new List<string>
            {
                KortrijkSpurs,
                JudoschoolKortrijk,
                LebadKortrijk,
                ReddersclubKortrijk,
                KoninklijkeTurnverenigingKortrijk,
                JongKortrijkVoetbalt,
                KortrijkseZwemkring,
                KortrijksSymfonischOrkest,
                KoninklijkKortrijkSportAtletiek,
                KortrijkseUltimateFrisbeeClub,
                RuygiKortrijk,
                RuygoJudoschoolKortrijk
            }.Select((x, i) => fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
              {
                  VCode = AssociationRegistry.Vereniging.VCode.Create(i + 32000),
                  Naam = x
              })
                    .Select(
                @event => @event with
                {
                    Locaties = [@event.Locaties.First() with { Adres = @event.Locaties.First().Adres with { Postcode = FictievePostcode } }]
                }
            ).ToList();
        }

    }

    private MultipleWerdenGeregistreerdWithGemeentenaamInVerenigingsnaamScenario _scenario;
    public List<ExpectedActual> Responses { get; private set; }

    public RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaamContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        _scenario = new();
    }

    public record ExpectedActual(RegistreerFeitelijkeVerenigingRequest request, string[] expected, string[] actual);

    public override async Task InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(_scenario);

        var autoFixture = new Fixture().CustomizeAdminApi();

        var request = autoFixture.Create<RegistreerFeitelijkeVerenigingRequest>();
        request.Locaties = autoFixture.CreateMany<ToeTeVoegenLocatie>().ToArray();
        request.Naam = "Ultimate Frisbee club";
        request.Locaties[0].Adres.Postcode = "8500";
        request.Locaties[0].Adres.Gemeente = "Kortrijk";

        var request2 = autoFixture.Create<RegistreerFeitelijkeVerenigingRequest>();
        request2.Locaties = autoFixture.CreateMany<ToeTeVoegenLocatie>().ToArray();
        request2.Naam = "Ryugi Kortrijk";
        request2.Locaties[0].Adres.Postcode = "8500";
        request2.Locaties[0].Adres.Gemeente = "Kortrijk";

        var requests = new List<ExpectedActual>
        {
            new(request, ["Ultimate Frisbee club Kortrijk"], []),
            new(request2, ["Ruygo Kortrijk"], []),
        };
        Responses = new List<ExpectedActual>();

        foreach (var r in requests)
        {
            var response = await (await ApiSetup.AdminApiHost.Scenario(s =>
            {
                s.Post
                 .Json(r.request, JsonStyle.Mvc)
                 .ToUrl("/v1/verenigingen/feitelijkeverenigingen");

                s.StatusCodeShouldBe(HttpStatusCode.Conflict);

                s.Header(WellknownHeaderNames.Sequence).ShouldNotBeWritten();
            })).ReadAsTextAsync();

            Responses.Add(r with { actual = ExtractDuplicateVerenigingsnamen(response).ToArray() });
        }

        await ApiSetup.AdminApiHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));


        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminProjectionHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.AllIndices);
    }

    private static IEnumerable<string> ExtractDuplicateVerenigingsnamen(string responseContent)
    {
        var duplicates = JObject.Parse(responseContent)
                                .SelectTokens("$.mogelijkeDuplicateVerenigingen[*].naam")
                                .Select(x => x.ToString());

        return duplicates;
    }

}
