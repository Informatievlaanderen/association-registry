namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using DuplicateVerenigingDetection;
using Infrastructure.HtmlValidation;
using System.Runtime.Serialization;

[DataContract]
public class VerenigingsType
{
    public VerenigingsType(
        string code,
        string naam)
    {
        Code = code;
        Naam = naam;
    }

    public static VerenigingsType FromDuplicaatVereniging(DuplicaatVereniging duplicaatVereniging)
        => new(duplicaatVereniging.Verenigingstype.Code, duplicaatVereniging.Verenigingstype.Naam);

    /// <summary>De code van het type van deze vereniging</summary>
    [DataMember(Name = "Code")]
    [NoHtml]
    public string Code { get; }

    /// <summary>De beschrijving van het type van deze vereniging</summary>
    [DataMember(Name = "Naam")]
    [NoHtml]
    public string Naam { get; }
}
