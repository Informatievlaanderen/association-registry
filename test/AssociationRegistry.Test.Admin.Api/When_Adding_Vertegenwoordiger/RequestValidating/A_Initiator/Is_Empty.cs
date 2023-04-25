namespace AssociationRegistry.Test.Admin.Api.When_Adding_Vertegenwoordiger.RequestValidating.A_Initiator;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VoegVertegenwoordigerToe;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__initiator_mag_niet_leeg_zijn()
    {
        var validator = new VoegVertegenwoordigerToeValidator();
        var result = validator.TestValidate(new VoegVertegenwoordigerToeRequest { Initiator = "" });

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Initiator)
            .WithErrorMessage("'Initiator' mag niet leeg zijn.")
            .Only();
    }
}
