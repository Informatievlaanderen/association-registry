namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning.RequestModels;

using System.Runtime.Serialization;

[DataContract]
public record TeRegistrerenErkenning
{
    [DataMember(Name = "ipdcProductNummer")]
    public string IpdcProductNummer { get; set; } = null!;

    [DataMember(Name = "Startdatum")]
    public DateOnly? Startdatum { get; set; }

    [DataMember(Name = "Einddatum")]
    public DateOnly? Einddatum { get; set; }

    [DataMember(Name = "Hernieuwingsdatum")]
    public DateOnly? Hernieuwingsdatum { get; set; }

    [DataMember(Name = "HernieuwingsUrl")]
    public string HernieuwingsUrl { get; set; } = null!;
}
