namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Contactgegeven.RequestValidating.Initiator;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VoegContactGegevenToe;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Valid : ValidatorTest
    {
        [Fact]
        public void Has_no_validation_errors_for_initiator()
        {
            var validator = new VoegContactgegevenToeValidator();
            var result = validator.TestValidate(new VoegContactgegevenToeRequest { Initiator = "abcd" });

            result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Initiator);
        }

        [Fact]
        public void Has_validation_errors_for_request()
        {
            var validator = new WijzigBasisgegevensRequestValidator();
            var result = validator.TestValidate(new WijzigBasisgegevensRequest() { Initiator = "abcd" });

            result.ShouldHaveValidationErrorFor("request");
        }
    }
