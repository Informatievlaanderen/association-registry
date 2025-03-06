namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging_With_Duplicates_With_Gemeentenaam_In_Verenigingsnaam.Beheer.Detail;

using Admin.Api.Infrastructure;
using Admin.Api.Verenigingen.Common;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

[Collection(WellKnownCollections.RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaam)]
public class Returns_Conflict : IClassFixture<RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaamContext>, IAsyncLifetime
{
    private readonly RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaamContext _context;
    private readonly ITestOutputHelper _testOutputHelper;

    public Returns_Conflict(RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaamContext context, ITestOutputHelper testOutputHelper)
    {
        _context = context;
        _testOutputHelper = testOutputHelper;
    }

    [Theory (Skip = "to replace with a singular duplicate test strategy")]
    [MemberData(nameof(Scenarios))]
    public async ValueTask WithDuplicateVerenigingen(RegistreerFeitelijkeVerenigingRequest request, string[] expectedDuplicateVerenigingen)
    {
        _testOutputHelper.WriteLine(request.Naam);
        var response = await (await _context.ApiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl("/v1/verenigingen/feitelijkeverenigingen");

            s.StatusCodeShouldBe(HttpStatusCode.Conflict);

            s.Header(WellknownHeaderNames.Sequence).ShouldNotBeWritten();
        })).ReadAsTextAsync();

        ExtractDuplicateVerenigingsnamen(response).Should().BeEquivalentTo(expectedDuplicateVerenigingen,
                                                                           because: $"'{request.Naam}' did not expect these duplicates");
    }


    public static IEnumerable<object[]> Scenarios()
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        yield return
        [
            RegistreerFeitelijkeVerenigingRequest(autoFixture, "Ultimate Frisbee club"),
            new[]
            {
                "Kortrijkse Ultimate Frisbee Club",
            },
        ];

        yield return
        [
            RegistreerFeitelijkeVerenigingRequest(autoFixture, "Ryugi Kortrijk"),
            new[]
            {
                "Ruygi KORTRIJK",
            },
        ];

        yield return
        [
            RegistreerFeitelijkeVerenigingRequest(autoFixture, "Judo School Kortrijk"),
            new[]
            {
                "JUDOSCHOOL KORTRIJK",
                "Ruygo Judoschool KORTRIJK"
            },
        ];

        yield return
        [
            RegistreerFeitelijkeVerenigingRequest(autoFixture, "Ryugi"),
            new[]
            {
                "Ruygi KORTRIJK",
            },
        ];

        yield return
        [
            RegistreerFeitelijkeVerenigingRequest(autoFixture, "Osu Judoschool Kortrijk"),
            new[]
            {
                "JUDOSCHOOL KORTRIJK",
                "Ruygo Judoschool KORTRIJK"
            },
        ];
    }

    private static RegistreerFeitelijkeVerenigingRequest RegistreerFeitelijkeVerenigingRequest(Fixture autoFixture, string verenigingsnaam)
    {
        var request = autoFixture.Create<RegistreerFeitelijkeVerenigingRequest>();
        request.Locaties = autoFixture.CreateMany<ToeTeVoegenLocatie>().ToArray();
        request.Naam = verenigingsnaam;
        request.Locaties[0].Adres.Postcode = "8500";
        request.Locaties[0].Adres.Gemeente = "FictieveGemeentenaam";

        return request;
    }

    private static IEnumerable<string> ExtractDuplicateVerenigingsnamen(string responseContent)
    {
        var duplicates = JObject.Parse(responseContent)
                                .SelectTokens("$.mogelijkeDuplicateVerenigingen[*].naam")
                                .Select(x => x.ToString());

        return duplicates;
    }

    public async ValueTask InitializeAsync()
        => await Task.CompletedTask;

    public async ValueTask DisposeAsync()
        => await Task.CompletedTask;
}
