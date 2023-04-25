namespace AssociationRegistry.Test.Admin.Api.When_Adding_Vertegenwoordiger.RequestValidating.A_Initiator;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VoegVertegenwoordigerToe;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new VoegVertegenwoordigerToeValidator();
        var result = validator.TestValidate(new VoegVertegenwoordigerToeRequest { Initiator = "abcd" });

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Initiator);
    }
}
