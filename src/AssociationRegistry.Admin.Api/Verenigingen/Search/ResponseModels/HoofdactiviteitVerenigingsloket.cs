namespace AssociationRegistry.Admin.Api.Verenigingen.Search.ResponseModels;

using Infrastructure.HtmlValidation;
using System.Runtime.Serialization;

[DataContract]
public class HoofdactiviteitVerenigingsloket
{
    /// <summary>
    /// De verkorte code van de hoofdactiviteit
    /// </summary>
    [DataMember]
    [NoHtml]
    public string Code { get; set; } = null!;

    /// <summary>
    /// De volledige beschrijving van de hoofdactiviteit
    /// </summary>
    [DataMember]
    [NoHtml]
    public string Naam { get; set; } = null!;
}
