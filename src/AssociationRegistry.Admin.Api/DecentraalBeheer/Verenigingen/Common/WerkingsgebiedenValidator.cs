namespace AssociationRegistry.Admin.Api.Verenigingen.Common;

using AssociationRegistry.Vereniging;
using FluentValidation;
using Infrastructure.WebApi.Validation;

public class WerkingsgebiedenValidator : AbstractValidator<string[]?>
{
    public WerkingsgebiedenValidator()
    {
        RuleFor(werkingsgebieden => werkingsgebieden)
           .Must(NotHaveDuplicates)
           .WithMessage(ValidationMessages.WerkingsgebiedMagSlechtsEenmaalVoorkomen)
           .When(werkingsgebieden => werkingsgebieden is not null);

        RuleFor(werkingsgebieden => werkingsgebieden)
           .Must(NotHaveMoreThanOne)
           .WithMessage(ValidationMessages.WerkingsgebiedKanNietGecombineerdWordenMetNVT)
           .When(werkingsgebieden => werkingsgebieden is not null && werkingsgebieden.Contains(Werkingsgebied.NietVanToepassing.Code));

        RuleForEach(werkingsgebieden => werkingsgebieden).MustNotContainHtml();
    }

    private static bool NotHaveDuplicates(string[]? values)
        => values is not null
            ? values.Length == values.DistinctBy(v => v.ToLower()).Count()
            : true;

    private static bool NotHaveMoreThanOne(string[]? values)
        => values is not null
            ? values.Length <= 1
            : true;
}
