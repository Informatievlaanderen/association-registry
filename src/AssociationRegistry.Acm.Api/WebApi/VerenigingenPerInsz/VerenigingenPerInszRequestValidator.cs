namespace AssociationRegistry.Acm.Api.WebApi.VerenigingenPerInsz;

using FluentValidation;

public class VerenigingenPerInszRequestValidator : AbstractValidator<VerenigingenPerInszRequest>
{
    public VerenigingenPerInszRequestValidator()
    {
        RuleFor(r => r.Insz)
           .NotEmpty()
           .WithMessage("'Insz' mag niet leeg zijn.")
           .NotNull()
           .WithMessage("'Insz' moet ingevuld zijn.");

        RuleForEach(r => r.KboNummers)
           .SetValidator(new KboNummerMetRechtsvormValidator())
           .When(r => r.KboNummers is not null && r.KboNummers.Any());
    }
}
