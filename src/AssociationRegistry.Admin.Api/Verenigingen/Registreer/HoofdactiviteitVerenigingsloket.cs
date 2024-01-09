namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using DuplicateVerenigingDetection;
using Infrastructure.HtmlValidation;
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
        DuplicaatVereniging.HoofdactiviteitVerenigingsloket hoofdactiviteitVerenigingsloket)
        => new(hoofdactiviteitVerenigingsloket.Code, hoofdactiviteitVerenigingsloket.Naam);

    /// <summary>De code van de hoofdactivititeit</summary>
    [DataMember(Name = "Code")]
    [NoHtml]
    public string Code { get; init; }

    /// <summary>De beschrijving van de hoofdactivititeit</summary>
    [DataMember(Name = "Naam")]
    [NoHtml]
    public string Naam { get; init; }
}
