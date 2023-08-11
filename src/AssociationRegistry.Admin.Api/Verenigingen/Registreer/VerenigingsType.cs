namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System.Runtime.Serialization;
using DuplicateVerenigingDetection;

[DataContract]
public class VerenigingsType
{
    public VerenigingsType(
        string code,
        string beschrijving)
    {
        Code = code;
        Beschrijving = beschrijving;
    }

    public static VerenigingsType FromDuplicaatVereniging(DuplicaatVereniging duplicaatVereniging)
        => new(duplicaatVereniging.Type.Code, duplicaatVereniging.Type.Beschrijving);

    /// <summary>De code van het type van deze vereniging</summary>
    [DataMember(Name = "Code")]
    public string Code { get; }

    /// <summary>De beschrijving van het type van deze vereniging</summary>
    [DataMember(Name = "Beschrijving")]
    public string Beschrijving { get; }
}