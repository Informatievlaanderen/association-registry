namespace AssociationRegistry.Shared.Serialization.JsonConverters;

using Newtonsoft.Json;
using System.Globalization;

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

        return DateOnly.ParseExact(readValue, _format, CultureInfo.InvariantCulture);
    }
}
