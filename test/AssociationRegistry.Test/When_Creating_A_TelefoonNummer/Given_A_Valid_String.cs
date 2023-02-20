﻿namespace AssociationRegistry.Test.When_Creating_A_TelefoonNummer;

using ContactInfo.TelefoonNummers;
using FluentAssertions;
using Xunit;

public class Given_A_Valid_String
{
    [Theory]
    [InlineData("0123456789")]
    [InlineData("0 1-2/3(4)5.6")]
    public void Then_it_creates_a_new_TelefoonNummer(string telefoonNummerString)
    {
        var telefoonNummer = TelefoonNummer.Create(telefoonNummerString);
        telefoonNummer.ToString().Should().BeEquivalentTo(telefoonNummerString);
    }
}
