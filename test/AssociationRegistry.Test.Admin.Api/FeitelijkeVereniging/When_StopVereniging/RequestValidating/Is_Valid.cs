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
public class Einddatum_Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_validation_errors_for_einddatum()
    {
        var validator = new StopVerenigingRequestValidator();
        var result = validator.TestValidate(new StopVerenigingRequest() { Einddatum = new DateOnly()});

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Einddatum);
    }
}
