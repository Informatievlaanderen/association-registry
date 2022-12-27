namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Json;

using Be.Vlaanderen.Basisregisters.Converters.TrimString;
using Be.Vlaanderen.Basisregisters.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Program;

public static class FormatterExtensions
{
    private static readonly DefaultContractResolver SharedContractResolver =
        DefaultApiJsonContractResolver.UsingDefaultNamingStrategy();

    /// <summary>
    /// Sets up and adds additional converters for an API to the JsonSerializerSettings
    /// </summary>
    /// <param name="source"></param>
    /// <returns>the updated JsonSerializerSettings</returns>
    public static JsonSerializerSettings ConfigureDefaultForApi(this JsonSerializerSettings source)
    {
        source.ContractResolver = SharedContractResolver;

        source.DateFormatHandling = DateFormatHandling.IsoDateFormat;
        source.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;

        var stringEnumConvertor = new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy()};
        stringEnumConvertor.NamingStrategy = new CamelCaseNamingStrategy(true, true);
        source.Converters.Add(stringEnumConvertor);

        source.Converters.Add(new TrimStringConverter());
        source.Converters.Add(new Rfc3339SerializableDateTimeOffsetConverter());

        return source
            .ConfigureForNodaTime(DateTimeZoneProviders.Tzdb)
            .WithIsoIntervalConverter();
    }
}
