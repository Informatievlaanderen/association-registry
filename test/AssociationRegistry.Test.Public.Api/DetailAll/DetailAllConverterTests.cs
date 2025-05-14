namespace AssociationRegistry.Test.Public.Api.DetailAll;

using AssociationRegistry.Public.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Public.Api.Verenigingen.DetailAll;
using AssociationRegistry.Public.Schema.Constants;
using AssociationRegistry.Public.Schema.Detail;
using AssociationRegistry.Test.Public.Api.Framework;
using AutoFixture;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json;
using Xunit;

public class DetailAllConverterTests
{
    private readonly Fixture _fixture;
    private readonly JsonSerializerSettings _serializerSettings;

    public DetailAllConverterTests()
    {
        _fixture = new Fixture().CustomizePublicApi();
        _serializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureDefaultForApi();
    }

    [Fact]
    public void Convert_Verwijderde_Vereniging()
    {
        var converter = new DetailAllConverter(new AppSettings());
        var doc = GetPubliekVerenigingDetailDocument();
        doc.Deleted = true;

        var actual = converter.SerializeToJson(doc);
        var expected = JsonConvert.SerializeObject(
            new DetailAllConverter.TeVerwijderenVereniging
            {
                Vereniging = new DetailAllConverter.TeVerwijderenVereniging.TeVerwijderenVerenigingData
                {
                    VCode = doc.VCode,
                    TeVerwijderen = true,
                    DeletedAt = doc.DatumLaatsteAanpassing,
                },
            },
            _serializerSettings);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Convert_UitgeschrevenUitPubliekeDatastroom_Vereniging()
    {
        var converter = new DetailAllConverter(new AppSettings());
        var doc = GetPubliekVerenigingDetailDocument();
        doc.IsUitgeschrevenUitPubliekeDatastroom = true;

        var actual = converter.SerializeToJson(doc);
        var expected = JsonConvert.SerializeObject(
            new DetailAllConverter.TeVerwijderenVereniging
            {
                Vereniging = new DetailAllConverter.TeVerwijderenVereniging.TeVerwijderenVerenigingData
                {
                    VCode = doc.VCode,
                    TeVerwijderen = true,
                    DeletedAt = doc.DatumLaatsteAanpassing,
                },
            },
            _serializerSettings);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Convert_Gestopte_Vereniging()
    {
        var converter = new DetailAllConverter(new AppSettings());
        var doc = GetPubliekVerenigingDetailDocument();
        doc.Status = VerenigingStatus.Gestopt;

        var actual = converter.SerializeToJson(doc);
        var expected = JsonConvert.SerializeObject(
            new DetailAllConverter.TeVerwijderenVereniging
            {
                Vereniging = new DetailAllConverter.TeVerwijderenVereniging.TeVerwijderenVerenigingData
                {
                    VCode = doc.VCode,
                    TeVerwijderen = true,
                    DeletedAt = doc.DatumLaatsteAanpassing,
                },
            },
            _serializerSettings);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Convert_Dubbele_Vereniging()
    {
        var converter = new DetailAllConverter(new AppSettings());
        var doc = GetPubliekVerenigingDetailDocument();
        doc.Status = VerenigingStatus.Dubbel;

        var actual = converter.SerializeToJson(doc);
        var expected = JsonConvert.SerializeObject(
            new DetailAllConverter.TeVerwijderenVereniging
            {
                Vereniging = new DetailAllConverter.TeVerwijderenVereniging.TeVerwijderenVerenigingData
                {
                    VCode = doc.VCode,
                    TeVerwijderen = true,
                    DeletedAt = doc.DatumLaatsteAanpassing,
                },
            },
            _serializerSettings);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Convert_Actieve_Vereniging()
    {
        var appSettings = new AppSettings();
        var converter = new DetailAllConverter(appSettings);
        var document = GetPubliekVerenigingDetailDocument();
        document.Deleted = false;

        var actual = converter.SerializeToJson(document);
        var expected = JsonConvert.SerializeObject(new PubliekVerenigingDetailAllMapper().Map(document, appSettings), _serializerSettings);

        actual.Should().BeEquivalentTo(expected);
    }

    private PubliekVerenigingDetailDocument GetPubliekVerenigingDetailDocument()
    {
        var doc = _fixture.Create<PubliekVerenigingDetailDocument>();
        doc.Status = VerenigingStatus.Actief;
        doc.IsUitgeschrevenUitPubliekeDatastroom = false;
        doc.Deleted = false;

        return doc;
    }

}
