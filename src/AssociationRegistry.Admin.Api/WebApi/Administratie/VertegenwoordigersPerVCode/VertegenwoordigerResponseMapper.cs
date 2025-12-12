namespace AssociationRegistry.Admin.Api.WebApi.Administratie.VertegenwoordigersPerVCode;

using Schema.Vertegenwoordiger;

public static class VertegenwoordigerResponseMapper
{
    public static VertegenwoordigerResponse[] Map(
        IEnumerable<VertegenwoordigersPerVCodeDocument> documents,
        string? status)
    {
        if (documents is null) throw new ArgumentNullException(nameof(documents));

        return documents
              .SelectMany(doc =>
               {
                   var vertegenwoordigers = doc.VertegenwoordigersData ?? Array.Empty<VertegenwoordigerData>();

                   if (!string.IsNullOrEmpty(status))
                   {
                       vertegenwoordigers = vertegenwoordigers
                                           .Where(v => v.Status == status)
                                           .ToArray();
                   }

                   return vertegenwoordigers.Select(v =>
                                                        new VertegenwoordigerResponse(
                                                            VCode: doc.VCode,
                                                            VertegenwoordigerId: v.VertegenwoordigerId,
                                                            Status: v.Status
                                                        ));
               })
              .ToArray();
    }
}
