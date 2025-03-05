namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging_With_Duplicates_With_Gemeentenaam_In_Verenigingsnaam;

using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using AutoFixture;
using Common.AutoFixture;
using Events;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens.FeitelijkeVereniging;
using Vereniging;

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
        public static string FictievePostcode = "8500";

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
                  VCode = VCode.Create(i + 32000),
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


    public RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaamContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
        ApiSetup = apiSetup;
        _scenario = new();
    }

    public override async ValueTask InitializeAsync()
    {
    }

    public override async ValueTask Init()    {
        await ApiSetup.ExecuteGiven(_scenario);

        await ApiSetup.AdminApiHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminProjectionHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.AllIndices);
    }
}
