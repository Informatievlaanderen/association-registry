namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging_With_Duplicates_With_Gemeentenaam_In_Verenigingsnaam.Beheer.Detail;

using Admin.Api.Infrastructure;
using Alba;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Common;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Net;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Conflict : IClassFixture<RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaamContext>, IAsyncLifetime
{
    private readonly RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaamContext _context;


    public Returns_Conflict(RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaamContext context)
    {
        _context = context;
    }

    [Theory]
    [MemberData(nameof(Scenarios))]
    public async Task WithDuplicateVerenigingen(RegistreerFeitelijkeVerenigingRequest request, string[] expectedDuplicateVerenigingen)
    {
        var response = await (await _context.ApiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl("/v1/verenigingen/feitelijkeverenigingen");

            s.StatusCodeShouldBe(HttpStatusCode.Conflict);

            s.Header(WellknownHeaderNames.Sequence).ShouldNotBeWritten();
        })).ReadAsTextAsync();

        ExtractDuplicateVerenigingsnamen(response).Should().BeEquivalentTo(expectedDuplicateVerenigingen);
    }


    public static IEnumerable<object[]> Scenarios()
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        yield return
        [
            RegistreerFeitelijkeVerenigingRequest(autoFixture, "Ultimate Frisbee club"),
            new[] { "Ultimate Frisbee club Kortrijk" },
        ];

        yield return
        [
            RegistreerFeitelijkeVerenigingRequest(autoFixture, "Ryugi Kortrijk"),
            new[] { "Ruygo Kortrijk" },
        ];
    }

    private static RegistreerFeitelijkeVerenigingRequest RegistreerFeitelijkeVerenigingRequest(Fixture autoFixture, string verenigingsnaam)
    {
        var request = autoFixture.Create<RegistreerFeitelijkeVerenigingRequest>();
        request.Locaties = autoFixture.CreateMany<ToeTeVoegenLocatie>().ToArray();
        request.Naam = verenigingsnaam;
        request.Locaties[0].Adres.Postcode = "AAAA";
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

    public async Task InitializeAsync()
        => await Task.CompletedTask;

    public async Task DisposeAsync()
        => await Task.CompletedTask;
}
