namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Websites.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Hernieuwingsurl
{
    private readonly WijzigErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask With_Invalid_Scheme_Then_Throws_WebsiteMoetStartenMetHttps()
    {
        var erkenning = _ctx.CreateTeWijzigenErkenning() with
        {
            HernieuwingsUrl = "ftp://example.com",
        };
        var command = _ctx.CreateCommand(teWijzigenErkenning: erkenning);

        var exception = await Assert.ThrowsAsync<WebsiteMoetStartenMetHttps>(async () => await _ctx.Handle(command));

        exception.Message.Should().Be(ExceptionMessages.InvalidWebsiteStart);
    }
}
