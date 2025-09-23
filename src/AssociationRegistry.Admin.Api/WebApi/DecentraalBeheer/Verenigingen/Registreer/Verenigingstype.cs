namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer;

using AssociationRegistry.Vereniging;
using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.DubbelDetectie;
using System.Runtime.Serialization;

[DataContract]
public class Verenigingstype : IVerenigingstype
{
    public Verenigingstype()
    { }

    public Verenigingstype(
        string code,
        string naam)
    {
        Code = code;
        Naam = naam;
    }

    public static Verenigingstype FromDuplicaatVereniging(DuplicaatVereniging duplicaatVereniging)
        => new(duplicaatVereniging.Verenigingstype.Code, duplicaatVereniging.Verenigingstype.Naam);

    /// <summary>De code van het type van deze vereniging</summary>
    [DataMember(Name = "Code")]
    public string Code { get; init; }

    /// <summary>De beschrijving van het type van deze vereniging</summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; }
}
