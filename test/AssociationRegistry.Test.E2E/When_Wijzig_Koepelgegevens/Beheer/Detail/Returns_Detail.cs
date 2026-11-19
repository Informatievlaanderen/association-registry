namespace AssociationRegistry.Test.E2E.When_Wijzig_Koepelgegevens.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;
using DomainKoepelstatus = DecentraalBeheer.Vereniging.Koepels.Koepelstatus;

[Collection(nameof(WijzigKoepelgegevensCollection))]
public class Returns_Detail : End2EndTest<DetailVerenigingResponse>
{
    private readonly WijzigKoepelgegevensContext _testContext;

    public Returns_Detail(WijzigKoepelgegevensContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetBeheerDetail(
            setup.AdminHttpClient,
            _testContext.CommandResult.VCode,
            headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)
        );

    [Fact]
    public void With_Koepelgegevens()
    {
        Response
            .Vereniging.Koepelgegevens.Should()
            .BeEquivalentTo(
                new Koepelgegevens
                {
                    Koepelstatus = DomainKoepelstatus.Koepel.Value,
                    PrivacyVerklaringGeaccepteerd = true,
                }
            );
    }
}
