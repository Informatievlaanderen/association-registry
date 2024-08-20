<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Detail/Projector/Given_EinddatumWerdGewijzigd.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Detail.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Detail/Projecting/Given_EinddatumWerdGewijzigd.cs

using AssociationRegistry.Admin.ProjectionHost.Constants;
using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_EinddatumWerdGewijzigd
{
    [Fact]
    public void Then_It_Changes_The_Einddatum()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var einddatumWerdGewijzigd = fixture.Create<TestEvent<EinddatumWerdGewijzigd>>();
        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(einddatumWerdGewijzigd, doc);

        doc.Einddatum.Should().Be(einddatumWerdGewijzigd.Data.Einddatum.ToString(WellknownFormats.DateOnly));
    }
}
