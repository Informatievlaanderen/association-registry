namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_TelefoonNummer;

using AssociationRegistry.Vereniging.TelefoonNummers;
using AssociationRegistry.Vereniging.TelefoonNummers.Exceptions;
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
        ctor.Should().Throw<TelefoonNummerBevatOngeldigeTekens>();
    }
}
