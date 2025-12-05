namespace AssociationRegistry.Test.Common.StubsMocksFakes;

using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using CommandHandling.DecentraalBeheer.Middleware;
using Magda.Persoon;

public class PersonenUitKszStub: PersonenUitKsz
{
    public PersonenUitKszStub(IEnumerable<PersoonUitKsz> personenUitKsz) : base(personenUitKsz)
    {
    }

    public PersonenUitKszStub(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand command, bool anyOverleden = false) : base(GetPersonenFromRegistreerVzerCommand(command, anyOverleden))
    {

    }

    private static IEnumerable<PersoonUitKsz> GetPersonenFromRegistreerVzerCommand(
        RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand command,
        bool anyOverleden)
    {
        return command.Vertegenwoordigers.Select(x =>
                                                     new PersoonUitKsz(x.Insz,
                                                                       x.Voornaam, x.Achternaam, anyOverleden));
    }
}
