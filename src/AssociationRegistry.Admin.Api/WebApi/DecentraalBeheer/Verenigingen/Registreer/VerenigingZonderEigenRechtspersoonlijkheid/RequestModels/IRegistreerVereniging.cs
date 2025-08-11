namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;

using AssociationRegistry.Vereniging;
using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using Common;
using DecentraalBeheer.Vereniging;

public interface IRegistreerVereniging
{
    /// <summary>Naam van de vereniging</summary>
    string Naam { get; set; }

    /// <summary>Korte naam van de vereniging</summary>
    string? KorteNaam { get; set; }

    /// <summary>Korte beschrijving van de vereniging</summary>
    string? KorteBeschrijving { get; set; }

    /// <summary>Datum waarop de vereniging gestart is. Deze datum mag niet later zijn dan vandaag</summary>
    DateOnly? Startdatum { get; set; }

    /// <summary>
    /// De doelgroep waar de activiteiten van deze vereniging zich op concentreert
    /// </summary>
    DoelgroepRequest? Doelgroep { get; set; }

    /// <summary>
    /// Is deze vereniging uitgeschreven uit de publieke datastroom
    /// </summary>
    bool IsUitgeschrevenUitPubliekeDatastroom { get; set; }

    /// <summary>De contactgegevens van deze vereniging</summary>
    ToeTeVoegenContactgegeven[] Contactgegevens { get; set; }

    /// <summary>Alle locaties waar deze vereniging actief is</summary>
    ToeTeVoegenLocatie[] Locaties { get; set; }

    /// <summary>De vertegenwoordigers van deze vereniging</summary>
    ToeTeVoegenVertegenwoordiger[] Vertegenwoordigers { get; set; }

    /// <summary>De codes van de hoofdactivititeiten volgens het verenigingsloket</summary>
    string[] HoofdactiviteitenVerenigingsloket { get; set; }

    /// <summary>De codes van de werkingsgebieden</summary>
    string[]? Werkingsgebieden { get; set; }

    RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand ToCommand(IWerkingsgebiedenService werkingsgebiedenService);
}
