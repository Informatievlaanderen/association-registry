namespace AssociationRegistry.Test.When_saving_a_vereniging;

using AutoFixture;
using Common.AutoFixture;
using Framework;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Duplicate_PhoneNumber
{
    [Theory]
    [InlineData("+32 412 34 56 78", "0032412345678")]
    [InlineData("+32 051 12 34 56", "003251123456")]
    [InlineData("+32 51 12 34 56", "003251123456")]
    [InlineData("+32 (412)/34.56.78", "0032412345678")]
    [InlineData("(+32) 0412 34 56 78", "0032412345678")]
    [InlineData("+32 412/34-56-78", "0032412345678")]
    [InlineData("+32412345678", "0032412345678")]
    [InlineData("+32 412 34 56 78 (home)", "0032412345678")]
    [InlineData("0412 34 56 78", "0032412345678")]
    [InlineData("0412345678", "0032412345678")]
    [InlineData("051 12 34 56", "003251123456")]
    [InlineData("051123456", "003251123456")]
    [InlineData("0032 412 34 56 78", "0032412345678")]
    [InlineData("+31 (0)20 369 0664", "0031203690664")]
    [InlineData("+4989 9982804-50", "004989998280450")]
    [InlineData("0032412345678", "+32 412 34 56 78")]
    [InlineData("003251123456", "+32 051 12 34 56")]
    [InlineData("003251123456", "+32 51 12 34 56")]
    [InlineData("0032412345678", "+32 (412)/34.56.78")]
    [InlineData("0032412345678", "(+32) 0412 34 56 78")]
    [InlineData("0032412345678", "+32 412/34-56-78")]
    [InlineData("0032412345678", "+32412345678")]
    [InlineData("0032412345678", "+32 412 34 56 78 (home)")]
    [InlineData("0032412345678", "0412 34 56 78")]
    [InlineData("0032412345678", "0412345678")]
    [InlineData("003251123456", "051 12 34 56")]
    [InlineData("003251123456", "051123456")]
    [InlineData("0032412345678", "0032 412 34 56 78")]
    [InlineData("0031203690664", "+31 (0)20 369 0664")]
    [InlineData("004989998280450", "+4989 9982804-50")]
    public void Then_Throws_ContactgegevenIsDuplicaatExceptions(string waarde, string otherWaarde)
    {
        var fixture = new Fixture().CustomizeDomain();
        var vCode = VCode.Create(1001);
        var naam = VerenigingsNaam.Create("Vereniging 1");
        const string beschrijving = "zelfde beschrijving";

        Assert.Throws<ContactgegevenIsDuplicaat>(() => Vereniging.RegistreerFeitelijkeVereniging(
                                                     vCode,
                                                     naam,
                                                     korteNaam: null,
                                                     korteBeschrijving: null,
                                                     startDatum: null,
                                                     Doelgroep.Null,
                                                     uitgeschrevenUitPubliekeDatastroom: false,
                                                     toeTeVoegenContactgegevens:
                                                     [
                                                         fixture.Create<Contactgegeven>() with
                                                         {
                                                             Beschrijving = beschrijving,
                                                             Contactgegeventype = Contactgegeventype.Telefoon,
                                                             Waarde = waarde,
                                                         },
                                                         fixture.Create<Contactgegeven>() with
                                                         {
                                                             Beschrijving = beschrijving,
                                                             Contactgegeventype = Contactgegeventype.Telefoon,
                                                             Waarde = waarde,
                                                         },
                                                     ],
                                                     Array.Empty<Locatie>(),
                                                     Array.Empty<Vertegenwoordiger>(),
                                                     Array.Empty<HoofdactiviteitVerenigingsloket>(),
                                                     new ClockStub(DateTime.Today)));
    }
}
