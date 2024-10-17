namespace AssociationRegistry.Acm.Api.VerenigingenPerInsz;

using System.Runtime.Serialization;

[DataContract]
public class VerenigingenPerInszRequest
{
    [DataMember]
    public string Insz { get; set; } = string.Empty;

    [DataMember]
    public KboRequest[] KboNummers { get; set; } = [];

    [DataContract]
    public class KboRequest
    {
        [DataMember]
        public string KboNummer { get; set; } = string.Empty;
        [DataMember]
        public string Rechtsvorm { get; set; } = string.Empty;
    }
}
