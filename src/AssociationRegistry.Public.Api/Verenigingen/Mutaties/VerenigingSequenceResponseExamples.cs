namespace AssociationRegistry.Public.Api.Verenigingen.Mutaties;

using Swashbuckle.AspNetCore.Filters;

public class VerenigingSequenceResponseExamples : IExamplesProvider<PubliekVerenigingSequenceResponse[]>
{
    public VerenigingSequenceResponseExamples()
    { }

    public PubliekVerenigingSequenceResponse[] GetExamples()
        =>
        [
            new PubliekVerenigingSequenceResponse()
            {
                VCode = "V0001021",
                Sequence = 10,
            },
            new PubliekVerenigingSequenceResponse()
            {
                VCode = "V0001052",
                Sequence = 15,
            },
        ];
}
