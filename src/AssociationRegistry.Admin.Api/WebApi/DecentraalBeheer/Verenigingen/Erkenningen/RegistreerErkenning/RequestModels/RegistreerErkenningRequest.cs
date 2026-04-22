namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning.RequestModels;

using System.Runtime.Serialization;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Erkenningen;

[DataContract]
public record RegistreerErkenningRequest
{
    /// <summary>De te registreren erkenning</summary>
    [DataMember(Name = "Erkenning")]
    public TeRegistrerenErkenning Erkenning { get; set; } = null!;

    public RegistreerErkenningCommand ToCommand(string vCode)
    {
        var erkenningsPeriode = ErkenningsPeriode.Create(Erkenning.Startdatum, Erkenning.Einddatum);

        return new RegistreerErkenningCommand(
            VCode.Create(vCode),
            new DecentraalBeheer.Vereniging.Erkenningen.TeRegistrerenErkenning()
            {
                IpdcProductNummer = Erkenning.IpdcProductNummer,
                ErkenningsPeriode = erkenningsPeriode,
                Hernieuwingsdatum = Hernieuwingsdatum.Create(Erkenning.Hernieuwingsdatum, erkenningsPeriode),
                HernieuwingsUrl = HernieuwingsUrl.Create(Erkenning.HernieuwingsUrl),
            }
        );
    }
}
