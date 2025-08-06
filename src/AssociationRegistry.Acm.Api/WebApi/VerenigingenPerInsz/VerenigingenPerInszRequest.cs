namespace AssociationRegistry.Acm.Api.WebApi.VerenigingenPerInsz;

using System.Runtime.Serialization;

[DataContract]
public class VerenigingenPerInszRequest
{
    /// <summary>
    /// Dit is de unieke identificatie van een persoon, dit kan een rijksregisternummer of bisnummer zijn
    /// </summary>
    [DataMember]
    public string Insz { get; set; } = string.Empty;

    /// <summary>
    /// Lijst van KBO-nummers met rechtsvorm gekoppeld aan deze persoon
    /// </summary>
    [DataMember]
    public KboNummerMetRechtsvormRequest[] KboNummers { get; set; } = [];

    /// <summary>
    /// KBO-nummer met rechtsvorm
    /// </summary>
    [DataContract]
    public class KboNummerMetRechtsvormRequest
    {
        /// <summary>
        /// KBO-nummer van een onderneming gekoppeld aan deze persoon
        /// </summary>
        [DataMember]
        public string KboNummer { get; set; } = string.Empty;

        /// <summary>
        /// Rechtsvorm van de onderneming
        /// </summary>
        [DataMember]
        public string Rechtsvorm { get; set; } = string.Empty;
    }
}
