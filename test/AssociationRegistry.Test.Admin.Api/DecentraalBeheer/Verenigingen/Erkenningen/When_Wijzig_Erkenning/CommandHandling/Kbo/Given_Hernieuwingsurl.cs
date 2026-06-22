namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging.Websites.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Hernieuwingsurl
{
    private readonly WijzigErkenningContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario> _ctx =
        new(new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask With_Invalid_Scheme_Then_Throws_WebsiteMoetStartenMetHttps()
    {
        var erkenning = _ctx.CreateTeWijzigenErkenning() with
        {
            HernieuwingsUrl = "ftp://example.com",
        };
        var command = _ctx.WijzigErkenningCommand with
        {
            Erkenning = erkenning,
        };

        var exception = await Assert.ThrowsAsync<WebsiteMoetStartenMetHttps>(async () => await _ctx.Handle(command));

        exception.Message.Should().Be(ExceptionMessages.InvalidWebsiteStart);
    }
}
