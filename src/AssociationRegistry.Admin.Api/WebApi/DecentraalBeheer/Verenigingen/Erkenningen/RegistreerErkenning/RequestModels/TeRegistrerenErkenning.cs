namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning.RequestModels;

using System.Runtime.Serialization;
using DecentraalBeheer.Vereniging.Erkenningen;

[DataContract]
public record TeRegistrerenErkenning
{
    [DataMember(Name = "ipdcProductNummer")]
    public string IpdcProductNummer { get; set; } = null!;

    [DataMember(Name = "Startdatum")]
    public DateOnly Startdatum { get; set; }

    [DataMember(Name = "Einddatum")]
    public DateOnly Einddatum { get; set; }

    [DataMember(Name = "Hernieuwingsdatum")]
    public DateOnly Hernieuwingsdatum { get; set; }

    [DataMember(Name = "HernieuwingsUrl")]
    public string HernieuwingsUrl { get; set; } = null!;

    public static AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.TeRegistrerenErkenning Map(
        TeRegistrerenErkenning erkenning
    ) =>
        new()
        {
            IpdcProductNummer = erkenning.IpdcProductNummer,
            Startdatum = erkenning.Startdatum,
            Einddatum = erkenning.Einddatum,
            Hernieuwingsdatum = erkenning.Hernieuwingsdatum,
            HernieuwingsUrl = erkenning.HernieuwingsUrl,
        };
}
