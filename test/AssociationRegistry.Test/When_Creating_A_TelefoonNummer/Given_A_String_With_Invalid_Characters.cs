namespace AssociationRegistry.Test.When_Creating_A_TelefoonNummer;

using ContactGegevens.TelefoonNummers;
using ContactGegevens.TelefoonNummers.Exceptions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_String_With_Invalid_Characters
{
    [Theory]
    [InlineData("********")]
    public void Then_it_throw_InvalidTelefoonNummerCharacterException(string invalidTelefoonNummer)
    {
        var ctor = () => TelefoonNummer.Create(invalidTelefoonNummer);
        ctor.Should().Throw<InvalidTelefoonNummerCharacter>();
    }
}
