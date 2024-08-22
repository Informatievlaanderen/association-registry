namespace AssociationRegistry.Test.Magda.GeefOndernemingService;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Constants;
using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Models.GeefOnderneming;
using AssociationRegistry.Magda.Onderneming.GeefOnderneming;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Hosts.Configuration.ConfigurationBindings;
using Kbo;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Geslaagd
{
    private readonly MagdaGeefVerenigingService _service;
    private readonly Fixture _fixture;
    private readonly string _verenigingNaam;
    private readonly string _verenigingKorteNaam;
    private readonly DateOnly _startDatum;
    private readonly AdresVolgensKbo _adres;
    private readonly ContactgegevensVolgensKbo _contactgegevens;

    public Given_Geslaagd()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _verenigingNaam = _fixture.Create<string>();
        _verenigingKorteNaam = _fixture.Create<string>();
        _startDatum = _fixture.Create<DateOnly>();
        _adres = _fixture.Create<AdresVolgensKbo>();
        _contactgegevens = _fixture.Create<ContactgegevensVolgensKbo>();

        var magdaFacade = new Mock<IMagdaClient>();
        var responseEnvelope = CreateResponseEnvelope(_verenigingNaam, _verenigingKorteNaam, _startDatum);

        magdaFacade.Setup(facade => facade.GeefOnderneming(It.IsAny<string>(), It.IsAny<MagdaCallReference>()))
                   .ReturnsAsync(responseEnvelope);

        _service = new MagdaGeefVerenigingService(Mock.Of<IMagdaCallReferenceRepository>(), magdaFacade.Object,
                                                  new TemporaryMagdaVertegenwoordigersSection(),
                                                  new NullLogger<MagdaGeefVerenigingService>());
    }

    private ResponseEnvelope<GeefOndernemingResponseBody> CreateResponseEnvelope(string naam, string korteNaam, DateOnly startDatum)
    {
        var responseEnvelope = _fixture.Create<ResponseEnvelope<GeefOndernemingResponseBody>>();

        responseEnvelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.Namen.MaatschappelijkeNamen = new[]
        {
            new NaamOndernemingType { Naam = naam, DatumBegin = "1990-01-01", Taalcode = TaalCodes.Nederlands },
        };

        responseEnvelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.Rechtsvormen = new[]
        {
            new RechtsvormExtentieType
            {
                Code = new CodeRechtsvormType
                {
                    Value = RechtsvormCodes.IVZW,
                },
            },
        };

        responseEnvelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.Namen.AfgekorteNamen = new[]
        {
            new NaamOndernemingType { Naam = korteNaam, DatumBegin = "1990-01-01", Taalcode = TaalCodes.Nederlands },
        };

        responseEnvelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.Start = new DatumStartType
        {
            Datum = startDatum.ToString(Formats.DateOnly),
        };

        responseEnvelope.Body.GeefOndernemingResponse.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.Adressen = new[]
        {
            new AdresOndernemingType
            {
                Type = new TypeAdresOndernemingType
                {
                    Code = new CodeTypeAdresOndernemingType
                    {
                        Value = AdresCodes.MaatschappelijkeZetel,
                    },
                },
                Descripties = new[]
                {
                    new DescriptieType
                    {
                        Adres = new AdresOndernemingBasisType
                        {
                            Straat = new StraatRR2_0Type
                            {
                                Naam = _adres.Straatnaam,
                            },
                            Huisnummer = _adres.Huisnummer,
                            Busnummer = _adres.Busnummer,
                            Gemeente = new GemeenteOptioneel2_0Type
                            {
                                PostCode = _adres.Postcode,
                                Naam = _adres.Gemeente,
                            },
                            Land = new Land2_0Type
                            {
                                Naam = _adres.Land,
                            },
                        },
                        Contact = new ContactType
                        {
                            Email = _contactgegevens.Email,
                            Website = _contactgegevens.Website,
                            Telefoonnummer = _contactgegevens.Telefoonnummer,
                            GSM = _contactgegevens.GSM,
                        },
                    },
                },
            },
        };

        return responseEnvelope;
    }

    [Fact]
    public async Task Then_It_Returns_A_SuccessResult()
    {
        var result = await _service.GeefVereniging(_fixture.Create<KboNummer>(), _fixture.Create<CommandMetadata>(),
                                                   CancellationToken.None);

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
            verenigingVolgensKbo.Type.Should().Be(Verenigingstype.IVZW);
            verenigingVolgensKbo.Naam.Should().Be(_verenigingNaam);
            verenigingVolgensKbo.KorteNaam.Should().Be(_verenigingKorteNaam);
            verenigingVolgensKbo.Startdatum.Should().Be(_startDatum); // before: BeEquivalent
            verenigingVolgensKbo.Adres.Should().BeEquivalentTo(_adres);
            verenigingVolgensKbo.Contactgegevens.Should().BeEquivalentTo(_contactgegevens);
        }
    }
}
