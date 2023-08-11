namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System.Runtime.Serialization;
using DuplicateVerenigingDetection;

[DataContract]
public class HoofdactiviteitVerenigingsloket
{
    public HoofdactiviteitVerenigingsloket(
        string code,
        string beschrijving)
    {
        Code = code;
        Beschrijving = beschrijving;
    }

    public static HoofdactiviteitVerenigingsloket FromDuplicaatVereniging(DuplicaatVereniging.HoofdactiviteitVerenigingsloket hoofdactiviteitVerenigingsloket)
        => new(hoofdactiviteitVerenigingsloket.Code, hoofdactiviteitVerenigingsloket.Beschrijving);

    /// <summary>De code van de hoofdactivititeit</summary>
    [DataMember(Name = "Code")]
    public string Code { get; init; }

    /// <summary>De beschrijving van de hoofdactivititeit</summary>
    [DataMember(Name = "Beschrijving")]
    public string Beschrijving { get; init; }
}