namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using AssociationRegistry.DuplicateVerenigingDetection;
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
    public string Code { get; }

    /// <summary>De beschrijving van het type van deze vereniging</summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; }
}
