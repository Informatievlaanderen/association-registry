namespace AssociationRegistry.Test.E2E.Data;

using System.Runtime.Serialization;

[DataContract]
public class TeVerwijderenVereniging
{
    [DataMember(Name = "vereniging")]
    public TeVerwijderenVerenigingData Vereniging { get; set; }
}

public class TeVerwijderenVerenigingData
{
    [DataMember(Name = "vcode")]
    public string VCode { get; set; }
    [DataMember(Name = "teVerwijderen")]
    public bool TeVerwijderen { get; set; }
}

