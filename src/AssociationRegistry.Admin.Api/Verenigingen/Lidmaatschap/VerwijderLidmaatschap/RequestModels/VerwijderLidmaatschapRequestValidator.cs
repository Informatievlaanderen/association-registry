namespace AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.VerwijderLidmaatschap.RequestModels;

using FluentValidation;

public class VerwijderLidmaatschapRequestValidator : AbstractValidator<VerwijderLidmaatschapRequest>
{
    public VerwijderLidmaatschapRequestValidator()
    {
        RuleFor(r => r.LidmaatschapId)
           .GreaterThan(0)
           .WithMessage(ValidationMessages.VeldIsVerplicht);
    }
}
