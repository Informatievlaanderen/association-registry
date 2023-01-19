namespace AssociationRegistry.Test.Admin.Api.When_validating.A_WijzigBasisgegevensRequest.Given_Initiator;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Valid : ValidatorTest
    {
        [Fact]
        public void Then_it_has_no_validation_errors_for_initiator()
        {
            var validator = new WijzigBasisgegevensRequestValidator();
            var result = validator.TestValidate(new WijzigBasisgegevensRequest() { Initiator = "abcd" });

            result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Initiator);
        }

        [Fact]
        public void Then_it_has_validation_errors_for_request()
        {
            var validator = new WijzigBasisgegevensRequestValidator();
            var result = validator.TestValidate(new WijzigBasisgegevensRequest() { Initiator = "abcd" });

            result.ShouldHaveValidationErrorFor("request");
        }
    }
