namespace AssociationRegistry.Acm.Api.VerenigingenPerInsz;

using System.Runtime.Serialization;

[DataContract]
public class VerenigingenPerInszRequest
{
    [DataMember]
    public string Insz { get; set; }

    [DataMember]
    public KboRequest[] KboNummers { get; set; }

    [DataContract]
    public class KboRequest
    {
        [DataMember]
        public string KboNummer { get; set; }
        [DataMember]
        public string Rechtsvorm { get; set; }
    }
}
