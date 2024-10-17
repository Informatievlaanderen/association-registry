namespace AssociationRegistry.Test.Acm.Api.Controllers;

using AssociationRegistry.Acm.Api.Queries.VerenigingenPerInsz;
using AssociationRegistry.Acm.Api.VerenigingenPerInsz;
using AssociationRegistry.Acm.Schema.VerenigingenPerInsz;
using AutoFixture;
using FluentAssertions;
using Framework;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Verenigingstype = AssociationRegistry.Acm.Api.VerenigingenPerInsz.Verenigingstype;

public class VerenigingenPerInszControllerTests
{
    [Fact]
    public async Task Given_No_Verenigingen_Per_Insz_Returns_Insz_With_Empty_Verenigingen()
    {
        var verenigingenPerInsz = new VerenigingenPerInszDocument()
        {
            Insz = "123",
            Verenigingen = [],
        };

        var verenigingenPerInszResponse = await GetVerenigingenPerInsz(verenigingenPerInsz);

        verenigingenPerInszResponse.Should().BeEquivalentTo(new VerenigingenPerInszResponse()
        {
            Insz = verenigingenPerInsz.Insz,
            Verenigingen = [],
        });
    }

    [Fact]
    public async Task Given_Verenigingen_Per_Insz_Returns_Insz_With_Verenigingen()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        var verenigingenPerInsz = new VerenigingenPerInszDocument()
        {
            Insz = "123",
            Verenigingen = fixture.CreateMany<Vereniging>().ToList(),
        };

        var verenigingenPerInszResponse = await GetVerenigingenPerInsz(verenigingenPerInsz);

        verenigingenPerInszResponse.Should().BeEquivalentTo(new VerenigingenPerInszResponse()
        {
            Insz = verenigingenPerInsz.Insz,
            Verenigingen = verenigingenPerInsz.Verenigingen.Select(s => new VerenigingenPerInszResponse.Vereniging()
            {
                VCode = s.VCode,
                CorresponderendeVCodes = s.CorresponderendeVCodes,
                VertegenwoordigerId = s.VertegenwoordigerId,
                Naam = s.Naam,
                Status = s.Status,
                KboNummer = s.KboNummer,
                Verenigingstype = new Verenigingstype(s.Verenigingstype.Code, s.Verenigingstype.Naam),
                IsHoofdvertegenwoordigerVan = s.IsHoofdvertegenwoordigerVan
            }).ToArray(),
        });
    }

    private async Task<VerenigingenPerInszResponse?> GetVerenigingenPerInsz(VerenigingenPerInszDocument verenigingenPerInsz)
    {
        var mockQuery = new Mock<IVerenigingenPerInszQuery>();
        var verenigingenPerInszRequest = new VerenigingenPerInszRequest(){Insz = verenigingenPerInsz.Insz};

        mockQuery.Setup(x => x.ExecuteAsync(
                            It.Is<VerenigingenPerInszFilter>(filter => filter.Insz == verenigingenPerInsz.Insz),
                            It.IsAny<CancellationToken>()))
                 .ReturnsAsync(verenigingenPerInsz);

        var sut = new VerenigingenPerInszController();

        var result = await sut.Get(mockQuery.Object, verenigingenPerInszRequest, CancellationToken.None);

        var okResult = result as OkObjectResult;
        var verenigingenPerInszResponse = okResult!.Value as VerenigingenPerInszResponse;

        return verenigingenPerInszResponse;
    }
}

