namespace AssociationRegistry.Test.Acm.Api.Controllers;

using AssociationRegistry.Acm.Api.Queries.VerenigingenPerInsz;
using AssociationRegistry.Acm.Api.VerenigingenPerInsz;
using AssociationRegistry.Acm.Schema.VerenigingenPerInsz;
using AssociationRegistry.AcmBevraging;
using AssociationRegistry.Magda.Constants;
using AutoFixture;
using FluentAssertions;
using Framework;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Verenigingstype = AssociationRegistry.Acm.Api.VerenigingenPerInsz.VerenigingenPerInszResponse.Verenigingstype;

public class VerenigingenPerInszControllerTests
{
    [Fact]
    public async ValueTask Given_No_Verenigingen_Returns_Insz_With_Empty_Verenigingen()
    {
        var verenigingenPerInsz = new VerenigingenPerInszDocument()
        {
            Insz = "123",
            Verenigingen = [],
        };

        VerenigingenPerKbo[] kboNummerInfos = [];

        var request = new VerenigingenPerInszRequest { Insz = verenigingenPerInsz.Insz };

        var verenigingenPerInszResponse = await GetVerenigingenPerInsz(verenigingenPerInsz, kboNummerInfos, request);

        verenigingenPerInszResponse.Should().BeEquivalentTo(new VerenigingenPerInszResponse()
        {
            Insz = verenigingenPerInsz.Insz,
            Verenigingen = [],
            KboNummers = [],
        });
    }

    [Fact]
    public async ValueTask Given_Verenigingen_Per_Kbo_Returns_Insz_With_KboNummers()
    {
        var verenigingenPerInsz = new VerenigingenPerInszDocument()
        {
            Insz = "123",
            Verenigingen = [],
        };

        VerenigingenPerKbo[] kboNummerInfos =
        [
            new("0987654321", "V0001001", true),
        ];

        var request = new VerenigingenPerInszRequest
        {
            Insz = verenigingenPerInsz.Insz,
            KboNummers =
            [
                new VerenigingenPerInszRequest.KboNummerMetRechtsvormRequest
                {
                    KboNummer = "0987654321",
                    Rechtsvorm = RechtsvormCodes.IVZW,
                },
            ],
        };

        var verenigingenPerInszResponse = await GetVerenigingenPerInsz(verenigingenPerInsz, kboNummerInfos, request);

        verenigingenPerInszResponse.Should().BeEquivalentTo(new VerenigingenPerInszResponse()
        {
            Insz = verenigingenPerInsz.Insz,
            Verenigingen = [],
            KboNummers = kboNummerInfos.Select(x => new VerenigingenPerInszResponse.VerenigingenPerKbo()
            {
                KboNummer = x.KboNummer,
                IsHoofdVertegenwoordiger = x.IsHoofdvertegenwoordiger,
                VCode = x.VCode,
            }).ToArray(),
        });
    }

    [Fact]
    public async ValueTask Given_Verenigingen_Per_Insz_Returns_Insz_With_Verenigingen()
    {
        var fixture = new Fixture().CustomizeAcmApi();

        var verenigingenPerInsz = new VerenigingenPerInszDocument()
        {
            Insz = "123",
            Verenigingen = fixture.CreateMany<Vereniging>()
                                  .Select(x =>
                                   {
                                       x.Verenigingssubtype = null;

                                       return x;
                                   }).ToList(),
        };

        VerenigingenPerKbo[] kboNummerInfos = [];

        var verenigingenPerInszRequest = new VerenigingenPerInszRequest { Insz = verenigingenPerInsz.Insz };

        var verenigingenPerInszResponse = await GetVerenigingenPerInsz(verenigingenPerInsz, kboNummerInfos, verenigingenPerInszRequest);

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
            KboNummers = [],
        });
    }

    private async Task<VerenigingenPerInszResponse?> GetVerenigingenPerInsz(
        VerenigingenPerInszDocument verenigingenPerInsz,
        VerenigingenPerKbo[] kboNummerInfos,
        VerenigingenPerInszRequest verenigingenPerInszRequest)
    {
        var mockVerenigingenPerInszQuery = new Mock<IVerenigingenPerInszQuery>();

        mockVerenigingenPerInszQuery.Setup(x => x.ExecuteAsync(
                                               It.Is<VerenigingenPerInszFilter>(filter => filter.Insz == verenigingenPerInsz.Insz),
                                               It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(verenigingenPerInsz);

        var kboNummersMetRechtsvorm = verenigingenPerInszRequest.KboNummers
                                                                .Select(s => new KboNummerMetRechtsvorm(s.KboNummer, s.Rechtsvorm))
                                                                .ToArray();

        var mockVerenigingenPerKboNummerService = new Mock<IVerenigingenPerKboNummerService>();
        mockVerenigingenPerKboNummerService.Setup(x => x.GetVerenigingenPerKbo(kboNummersMetRechtsvorm, It.IsAny<CancellationToken>()))
                                           .ReturnsAsync(kboNummerInfos);

        var sut = new VerenigingenPerInszController();

        var result = await sut.Post(mockVerenigingenPerInszQuery.Object, mockVerenigingenPerKboNummerService.Object,
                                   verenigingenPerInszRequest, CancellationToken.None);

        var okResult = result as OkObjectResult;
        var verenigingenPerInszResponse = okResult!.Value as VerenigingenPerInszResponse;

        return verenigingenPerInszResponse;
    }
}
