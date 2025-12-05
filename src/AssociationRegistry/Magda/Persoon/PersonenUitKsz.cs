namespace AssociationRegistry.Magda.Persoon;

using System.Collections.ObjectModel;

public class PersonenUitKsz : ReadOnlyCollection<PersoonUitKsz>
{

    public PersonenUitKsz(IEnumerable<PersoonUitKsz> personenUitKsz) : base(personenUitKsz.ToList())
    {
    }

#pragma warning disable CS0108, CS0114
    public static readonly PersonenUitKsz Empty = new PersonenUitKsz([]);
#pragma warning restore CS0108, CS0114

    public bool HeeftOverledenPersonen => this.Any(x => x.Overleden);
}

public record PersoonUitKsz(string Insz, string Voornaam, string Achternaam, bool Overleden);
