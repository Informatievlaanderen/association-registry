namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.SchorsErkenning.RequestModels;

using System.Runtime.Serialization;

[DataContract]
public record TeSchorsenErkenning
{
    /// <summary>
    /// Reden waarom de erkenning geschorst wordt.
    /// </summary>
    [DataMember(Name = "redenSchorsing")]
    public string RedenSchorsing { get; set; }
}
