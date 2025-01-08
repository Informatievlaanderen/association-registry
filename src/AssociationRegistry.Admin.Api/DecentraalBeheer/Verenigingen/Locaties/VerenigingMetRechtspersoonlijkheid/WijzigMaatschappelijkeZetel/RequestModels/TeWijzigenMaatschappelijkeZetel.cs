namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;

using Acties.Locaties.WijzigMaatschappelijkeZetel;
using AssociationRegistry.Vereniging;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

[DataContract]
public class TeWijzigenMaatschappelijkeZetel
{
    /// <summary>Duidt aan dat dit de primaire locatie is</summary>
    [DataMember]
    public bool? IsPrimair { get; set; }

    /// <summary>Een beschrijvende naam voor de locatie</summary>
    [DataMember]
    [MaxLength(Locatie.MaxLengthLocatienaam)]
    public string? Naam { get; set; }

    public static WijzigMaatschappelijkeZetelCommand.Locatie Map(TeWijzigenMaatschappelijkeZetel locatie, int locatieId)
        => new(
            locatieId,
            locatie.IsPrimair,
            locatie.Naam);
}
