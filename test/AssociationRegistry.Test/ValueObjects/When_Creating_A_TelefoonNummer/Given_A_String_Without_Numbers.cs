namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_TelefoonNummer;

using AssociationRegistry.Vereniging.TelefoonNummers;
using AssociationRegistry.Vereniging.TelefoonNummers.Exceptions;
using FluentAssertions;
using Xunit;

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
