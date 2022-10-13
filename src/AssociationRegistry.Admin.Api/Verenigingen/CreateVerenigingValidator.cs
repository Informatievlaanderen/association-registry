namespace AssociationRegistry.Admin.Api.Verenigingen;

using FluentValidation;

public class CreateVerenigingValidator : AbstractValidator<CommandEnvelope<CreateVerenigingCommand>>
{
    public CreateVerenigingValidator()
    {
        RuleFor(c => c.Command.Naam).NotNull().NotEmpty().WithMessage("Naam is verplicht");
    }
}
