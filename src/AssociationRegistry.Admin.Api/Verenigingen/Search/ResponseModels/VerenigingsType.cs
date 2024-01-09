namespace AssociationRegistry.Admin.Api.Verenigingen.Search.ResponseModels;

using Infrastructure.HtmlValidation;
using System.Runtime.Serialization;

[DataContract]
public class VerenigingsType
{
    /// <summary>
    /// De code van het type vereniging
    /// </summary>
    [DataMember]
    [NoHtml]
    public string Code { get; set; } = null!;

    /// <summary>
    /// De beschrijving van het type vereniging
    /// </summary>
    [DataMember]
    [NoHtml]
    public string Naam { get; set; } = null!;
}
