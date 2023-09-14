namespace AssociationRegistry.Test.When_Creating_A_TelefoonNummer;

using FluentAssertions;
using Vereniging.TelefoonNummers;
using Vereniging.TelefoonNummers.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_String_Without_Numbers
{
    [Theory]
    [InlineData("/(/-...-/")]
    public void Then_it_throw_NoNumbersInTelefoonNummerException(string invalidTelefoonNummer)
    {
        var ctor = () => TelefoonNummer.Create(invalidTelefoonNummer);
        ctor.Should().Throw<TelefoonNummerMoetCijferBevatten>();
    }
}
