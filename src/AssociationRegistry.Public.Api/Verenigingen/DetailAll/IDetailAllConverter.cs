namespace AssociationRegistry.Public.Api.Verenigingen.DetailAll;

using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
using Infrastructure.ConfigurationBindings;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json;
using Schema.Constants;
using Schema.Detail;

public interface IDetailAllConverter
{
    string SerializeToJson(PubliekVerenigingDetailDocument publiekVerenigingDetailDocument);
}

public class DetailAllConverter : IDetailAllConverter
{
    private readonly AppSettings _appSettings;
    private readonly JsonSerializerSettings _serializerSettings;

    public DetailAllConverter(AppSettings appSettings)
    {
        _appSettings = appSettings;
        _serializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureDefaultForApi();
    }

    public string SerializeToJson(PubliekVerenigingDetailDocument vereniging)
    {
        if (IsTeVerwijderenVereniging(vereniging))
            return
                JsonConvert.SerializeObject(
                    new TeVerwijderenVereniging
                    {
                        Vereniging = new TeVerwijderenVereniging.TeVerwijderenVerenigingData
                        {
                            VCode = vereniging.VCode,
                            TeVerwijderen = true,
                            DeletedAt = vereniging.DatumLaatsteAanpassing,
                        },
                    },
                    _serializerSettings);

        var mappedVereniging = PubliekVerenigingDetailAllMapper.Map(vereniging, _appSettings);

        return JsonConvert.SerializeObject(mappedVereniging, _serializerSettings);
    }

    private static bool IsTeVerwijderenVereniging(PubliekVerenigingDetailDocument vereniging)
        => vereniging.Deleted ||
           vereniging.Status == VerenigingStatus.Gestopt ||
           vereniging.Status == VerenigingStatus.Dubbel ||
           vereniging.IsUitgeschrevenUitPubliekeDatastroom is not null &&
           vereniging.IsUitgeschrevenUitPubliekeDatastroom.Value;

    public class TeVerwijderenVereniging
    {
        public TeVerwijderenVerenigingData Vereniging { get; set; }

        public class TeVerwijderenVerenigingData
        {
            public string VCode { get; set; }
            public bool TeVerwijderen { get; set; }
            public string DeletedAt { get; set; }
        }
    }
}
