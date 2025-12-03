namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Middleware;

using System.Collections.ObjectModel;

public class PersonenUitKsz : ReadOnlyCollection<PersoonUitKsz>
{
    private readonly IEnumerable<PersoonUitKsz> _personenUitKsz;

    public PersonenUitKsz(IEnumerable<PersoonUitKsz> personenUitKsz) : base(personenUitKsz.ToList())
    {
    }

    public bool HeeftOverledenPersonen => this.Any(x => x.Overleden);
}

public record PersoonUitKsz(string Insz, string Voornaam, string Achternaam, bool Overleden);
