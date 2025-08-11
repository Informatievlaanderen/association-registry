namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer;

using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using System.Runtime.Serialization;

[DataContract]
public class HoofdactiviteitVerenigingsloket
{
    public HoofdactiviteitVerenigingsloket(
        string code,
        string naam)
    {
        Code = code;
        Naam = naam;
    }

    public static HoofdactiviteitVerenigingsloket FromDuplicaatVereniging(
        DuplicaatVereniging.Types.HoofdactiviteitVerenigingsloket hoofdactiviteitVerenigingsloket)
        => new(hoofdactiviteitVerenigingsloket.Code, hoofdactiviteitVerenigingsloket.Naam);

    /// <summary>De code van de hoofdactivititeit</summary>
    [DataMember(Name = "Code")]
    public string Code { get; init; }

    /// <summary>De beschrijving van de hoofdactivititeit</summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; }
}
