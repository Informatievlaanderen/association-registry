namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_WijzigBasisGegevens.RequestValidating.Startdatum;

using AssociationRegistry.Admin.Api.Verenigingen.Stop;
using AssociationRegistry.Admin.Api.Verenigingen.Stop.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using Primitives;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Einddatum_Is_Null : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors_for_startdatum()
    {
        var validator = new StopVerenigingRequestValidator();
        var result = validator.TestValidate(new StopVerenigingRequest { Einddatum = default});

        result.ShouldHaveValidationErrorFor(nameof(WijzigBasisgegevensRequest.Startdatum));
    }
}
