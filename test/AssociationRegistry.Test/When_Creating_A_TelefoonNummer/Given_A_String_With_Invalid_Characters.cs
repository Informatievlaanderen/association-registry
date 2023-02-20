﻿namespace AssociationRegistry.Test.When_Creating_A_TelefoonNummer;

using ContactInfo.TelefoonNummers;
using ContactInfo.TelefoonNummers.Exceptions;
using FluentAssertions;
using Xunit;

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
