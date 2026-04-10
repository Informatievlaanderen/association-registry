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

    public RegistreerErkenningCommand ToCommand(string vCode) =>
        new(
            VCode.Create(vCode),
            new DecentraalBeheer.Vereniging.Erkenningen.TeRegistrerenErkenning()
            {
                IpdcProduct = new IpdcProduct() { Nummer = Erkenning.IpdcProductNummer },
                Startdatum = Erkenning.Startdatum,
                Einddatum = Erkenning.Einddatum,
                Hernieuwingsdatum = Erkenning.Hernieuwingsdatum,
                HernieuwingsUrl = Erkenning.HernieuwingsUrl,
            }
        );
}
