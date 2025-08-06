namespace AssociationRegistry.MartenDb.Serialization;

using Newtonsoft.Json;
using System.Globalization;

public class NullableDateOnlyJsonConvertor : JsonConverter<DateOnly?>
{
    private readonly string _format;

    public NullableDateOnlyJsonConvertor(string format)
    {
        _format = format;
    }

    public override void WriteJson(JsonWriter writer, DateOnly? date, JsonSerializer serializer)
        => writer.WriteValue(date?.ToString(_format, CultureInfo.InvariantCulture));

    public override DateOnly? ReadJson(
        JsonReader reader,
        Type objectType,
        DateOnly? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        var readValue = (string)reader.Value!;

        if (string.IsNullOrEmpty(readValue)) return null;

        return DateOnlyHelpers.Parse(readValue, _format);
    }
}
