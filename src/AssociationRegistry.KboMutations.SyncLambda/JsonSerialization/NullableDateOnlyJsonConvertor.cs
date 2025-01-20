using System.Globalization;
using System.Runtime.Serialization;
using AssociationRegistry.Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

namespace AssociationRegistry.KboMutations.SyncLambda.JsonSerialization;

public class NullableDateOnlyJsonConvertor : JsonConverter<DateOnly?>
{
    private readonly string _format;

    public NullableDateOnlyJsonConvertor(string format)
    {
        _format = format;
    }

    public override void WriteJson(JsonWriter writer, DateOnly? value, JsonSerializer serializer)
        => writer.WriteValue(value.HasValue ? value.Value.ToString(_format, CultureInfo.InvariantCulture) : string.Empty);

    public override DateOnly? ReadJson(
        JsonReader reader,
        Type objectType,
        DateOnly? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        var readValue = (string)reader.Value!;

        if (string.IsNullOrEmpty(readValue)) return null;

        return DateOnlyHelpers.TryParse(readValue, _format);
    }
}

[Serializable]
public class InvalidDateFormat : DomainException
{
    public InvalidDateFormat() : base(ExceptionMessages.InvalidDateFormat)
    {
    }

    protected InvalidDateFormat(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

public static class WellknownFormats
{
    public const string DateOnly = "yyyy-MM-dd";
    public static readonly CultureInfo Belgie = CultureInfo.GetCultureInfo("nl-BE");
}
