namespace AssociationRegistry.Test.Admin.Api.When_instantiating_a_KboNummer;

using AssociationRegistry.KboNummers;
using AssociationRegistry.KboNummers.Exceptions;
using Xunit;

public class Given_a_number_that_fails_the_checksum
{
    [Fact]
    public void Then_it_throws_an_exception()
    {
        Assert.Throws<InvalidKboNummerMod97>(() => KboNummer.Create("0123456789"));
    }
}
