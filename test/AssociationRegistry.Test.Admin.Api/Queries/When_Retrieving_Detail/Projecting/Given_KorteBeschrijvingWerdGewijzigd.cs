<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Detail/Projector/Given_KorteBeschrijvingWerdGewijzigd.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Detail.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Detail/Projecting/Given_KorteBeschrijvingWerdGewijzigd.cs

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_KorteBeschrijvingWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_korteBeschrijving()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var korteBeschrijvingWerdGewijzigd = fixture.Create<TestEvent<KorteBeschrijvingWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(korteBeschrijvingWerdGewijzigd, doc);

        doc.KorteBeschrijving.Should().Be(korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving);
    }
}
