namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Contactgegeven;

using AssociationRegistry.Vereniging.TelefoonNummers;
using Xunit;

public class With_Different_PhoneNumbers
{
    [Theory]
    [InlineData("+32 412 34 56 78", "0032412345678")]
    [InlineData("+32 051 12 34 56", "003251123456")]
    [InlineData("+32 51 12 34 56", "003251123456")]
    [InlineData("+32 (412)/34.56.78", "0032412345678")]
    [InlineData("(+32) 0412 34 56 78", "0032412345678")]
    [InlineData("+32 412/34-56-78", "0032412345678")]
    [InlineData("+32412345678", "0032412345678")]
    [InlineData("+32412345678 ()", "0032412345678")]
    [InlineData("+32 412 34 56 78 (home)", "0032412345678")]
    [InlineData("0412 34 56 78", "0032412345678")]
    [InlineData("0412345678", "0032412345678")]
    [InlineData("051 12 34 56", "003251123456")]
    [InlineData("051123456", "003251123456")]
    [InlineData("0032 412 34 56 78", "0032412345678")]
    [InlineData("+31 (0)20 369 0664", "0031203690664")]
    [InlineData("+4989 9982804-50", "004989998280450")]
    public void NormalizePhoneNumber_ShouldReturnCorrectFormat(string input, string expected)
    {
        var actual = TelefoonNummer.NormalizePhoneNumber(input);

        Assert.Equal(expected, actual);
    }
}
