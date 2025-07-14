namespace AssociationRegistry.Shared.Serialization.JsonConverters;

using Newtonsoft.Json;
using System.Globalization;

public class DateOnlyJsonConvertor : JsonConverter<DateOnly>
{
    private readonly string _format;
    private readonly bool _allowEmptyString;

    public DateOnlyJsonConvertor(string format, bool allowEmptyString = false)
    {
        _format = format;
        _allowEmptyString = allowEmptyString;
    }

    public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString(_format, CultureInfo.InvariantCulture));
    }

    public override DateOnly ReadJson(
        JsonReader reader,
        Type objectType,
        DateOnly existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
        => reader.Value!.Equals(string.Empty) && _allowEmptyString
            ? DateOnly.MinValue
            : DateOnlyHelpers.TryParseExactOrThrow((string)reader.Value!, _format);
}
