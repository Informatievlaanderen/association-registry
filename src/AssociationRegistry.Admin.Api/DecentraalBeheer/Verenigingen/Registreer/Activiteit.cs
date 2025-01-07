namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Registreer;

using AssociationRegistry.DuplicateVerenigingDetection;
using System.Runtime.Serialization;

/// <summary>Een activiteit van een vereniging</summary>
[DataContract]
public class Activiteit
{
    public Activiteit(
        int id,
        string categorie)
    {
        Id = id;
        Categorie = categorie;
    }

    public static Activiteit FromDuplicaatVereniging(DuplicaatVereniging.Activiteit locatie)
        => new(locatie.Id, locatie.Categorie);

    /// <summary>De unieke identificatie code van deze activiteit binnen de vereniging</summary>
    [DataMember(Name = "Id")]
    public int Id { get; init; }

    /// <summary>De categorie van deze activiteit</summary>
    [DataMember(Name = "Categorie")]
    public string Categorie { get; init; }
}
