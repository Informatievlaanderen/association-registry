namespace AssociationRegistry.Test.Admin.Api.Magda.MagdaKboService.When_Retrieving_VerenigingVolgensKbo;

using AssociationRegistry.Admin.Api.Magda;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Constants;
using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Models.GeefOnderneming;
using AssociationRegistry.Magda.Onderneming.GeefOnderneming;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Framework;
using Kbo;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_GeefOndernemingResponseBody
{
    private readonly MagdaGeefVerenigingService _service;
    private readonly Fixture _fixture;
    private readonly string _verenigingNaam;
    private readonly string _verenigingKorteNaam;
    private readonly DateOnly _startDatum;
    private readonly ResponseEnvelope<GeefOndernemingResponseBody> _responseEnvelope;

    public Given_A_GeefOndernemingResponseBody()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _verenigingNaam = _fixture.Create<string>();
        _verenigingKorteNaam = _fixture.Create<string>();
        _startDatum = _fixture.Create<DateOnly>();

        var magdaFacade = new Mock<IMagdaFacade>();
        _responseEnvelope = CreateResponseEnvelope(_verenigingNaam, _verenigingKorteNaam, _startDatum);
        magdaFacade.Setup(facade => facade.GeefOnderneming(It.IsAny<string>(), It.IsAny<MagdaCallReference>()))
            .ReturnsAsync(_responseEnvelope);

        _service = new MagdaGeefVerenigingService(Mock.Of<IMagdaCallReferenceRepository>(), magdaFacade.Object, new NullLogger<MagdaGeefVerenigingService>());
    }

    private ResponseEnvelope<GeefOndernemingResponseBody> CreateResponseEnvelope(string naam, string korteNaam, DateOnly startDatum)
    {
        var responseEnvelope = _fixture.Create<ResponseEnvelope<GeefOndernemingResponseBody>>();

        responseEnvelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.Namen.MaatschappelijkeNamen = new[]
        {
            new NaamOndernemingType { Naam = naam, DatumBegin = "1990-01-01", Taalcode = TaalCodes.Nederlands },
        };
        responseEnvelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.Namen.AfgekorteNamen = new[]
        {
            new NaamOndernemingType { Naam = korteNaam, DatumBegin = "1990-01-01", Taalcode = TaalCodes.Nederlands },
        };
        responseEnvelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.Start = new DatumStartType
        {
            Datum = startDatum.ToString(Formats.DateOnly),
        };
        return responseEnvelope;
    }

    [Fact]
    public async Task Then_It_Returns_A_SuccessResult()
    {
        var result = await _service.GeefVereniging(_fixture.Create<KboNummer>(), _fixture.Create<CommandMetadata>(), CancellationToken.None);
        result.IsSuccess().Should().BeTrue();
    }

    [Fact]
    public async Task Then_It_Returns_A_VerenigingVolgensKbo()
    {
        var kboNummer = _fixture.Create<KboNummer>();
        var result = await _service.GeefVereniging(kboNummer, _fixture.Create<CommandMetadata>(), CancellationToken.None);
        using (new AssertionScope())
        {
            var verenigingVolgensKbo = result.Should().BeOfType<Result<VerenigingVolgensKbo>>().Subject.Data;
            verenigingVolgensKbo.KboNummer.Should().BeEquivalentTo(kboNummer);
            verenigingVolgensKbo.Rechtsvorm.CodeVolgensMagda.Should().Be(_responseEnvelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.Rechtsvormen.First().Code.Value);
            verenigingVolgensKbo.Naam.Should().Be(_verenigingNaam);
            verenigingVolgensKbo.KorteNaam.Should().Be(_verenigingKorteNaam);
            verenigingVolgensKbo.StartDatum.Should().BeEquivalentTo(_startDatum);
        }
    }
}
