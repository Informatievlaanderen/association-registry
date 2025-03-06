namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Mapping_LocatieIdsPerVCode;

using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Grar.GrarConsumer.Messaging.HeradresseerLocaties;
using Grar.GrarUpdates.Hernummering;
using Grar.GrarUpdates.LocatieFinder;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class Given_Locaties_For_Different_VCodes
{
    [Fact]
    public void Then_Returns_HeradresseerLocatiesMessage_For_VCode()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var destinationAdresId = fixture.Create<int>();
        var idempotencyKey = fixture.Create<string>();

        var vCode1 = fixture.Create<string>();
        var locatieIdsForVCode1 = fixture.Create<LocatieLookupData[]>();

        var vCode2 = fixture.Create<string>();
        var locatieIdsForVCode2 = fixture.Create<LocatieLookupData[]>();

        var locatieIdsPerVCode = LocatiesPerVCodeCollection.FromLocatiesPerVCode(new Dictionary<string, LocatieLookupData[]>()
        {
            { vCode1, locatieIdsForVCode1 },
            { vCode2, locatieIdsForVCode2 },
        });

        var actual = locatieIdsPerVCode.Map(destinationAdresId, idempotencyKey);

        actual.Should().BeEquivalentTo([
            new HeradresseerLocatiesMessage(
                vCode1,
                locatieIdsForVCode1.Select(l => new TeHeradresserenLocatie(l.LocatieId, destinationAdresId.ToString())).ToList(),
                idempotencyKey),
            new HeradresseerLocatiesMessage(
                vCode2,
                locatieIdsForVCode2.Select(l => new TeHeradresserenLocatie(l.LocatieId, destinationAdresId.ToString())).ToList(),
                idempotencyKey),
        ]);
    }
}
