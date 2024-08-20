<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Detail/Projector/Given_HoofdactiviteitenVerenigingsregisterWerdenGewijzigd.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Detail.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Detail/Projecting/Given_HoofdactiviteitenVerenigingsregisterWerdenGewijzigd.cs

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_HoofdactiviteitenVerenigingsregisterWerdenGewijzigd
{
    [Fact]
    public void Then_it_replaces_the_hoofdactiviteitenVerenigingsLoket()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var hoofdactiviteitenVerenigingsloketWerdenGewijzigd =
            fixture.Create<TestEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(hoofdactiviteitenVerenigingsloketWerdenGewijzigd, doc);

        doc.HoofdactiviteitenVerenigingsloket.Should()
           .BeEquivalentTo(hoofdactiviteitenVerenigingsloketWerdenGewijzigd.Data.HoofdactiviteitenVerenigingsloket);
    }
}
