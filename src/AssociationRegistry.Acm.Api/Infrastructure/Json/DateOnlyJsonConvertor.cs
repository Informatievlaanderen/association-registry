namespace AssociationRegistry.Acm.Api.Infrastructure.Json;

using Newtonsoft.Json;
using System;
using System.Globalization;

public class DateOnlyJsonConvertor : JsonConverter<DateOnly>
{
    private readonly string _format;

    public DateOnlyJsonConvertor(string format)
    {
        _format = format;
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
        => DateOnlyHelpers.TryParse((string)reader.Value!, _format);
}
