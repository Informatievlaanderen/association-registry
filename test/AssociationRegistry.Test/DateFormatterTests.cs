namespace AssociationRegistry.Test;

using AssociationRegistry.Formats;
using FluentAssertions;
using NodaTime;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

public class DateFormatterTests
{
    private readonly ITestOutputHelper _output;

    public DateFormatterTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Then_It_Should_Convert_And_Format_ToBelgianDateFormat_Also()
    {
        var winterInstant = Instant.FromDateTimeOffset(DateTimeOffset.Parse("2024-01-01 00:00:00.000000+00"));

        _output.WriteLine(winterInstant.ToString());
        _output.WriteLine(winterInstant.ConvertAndFormatToBelgianDate());
        _output.WriteLine(winterInstant.ConvertAndFormatToBelgianTime());

        winterInstant.ConvertAndFormatToBelgianTime().Should().Be("01:00:00");

        var summerInstant = Instant.FromDateTimeOffset(DateTimeOffset.Parse("2024-08-01 00:00:00.000000+00"));

        _output.WriteLine(summerInstant.ToString());
        _output.WriteLine(summerInstant.ConvertAndFormatToBelgianDate());
        _output.WriteLine(summerInstant.ConvertAndFormatToBelgianTime());

        summerInstant.ConvertAndFormatToBelgianTime().Should().Be("02:00:00");
    }

}
