---
name: migrate-erkenningen-commandhandling-tests
description: Migrate a specific erkenningen CommandHandling/ folder from the old Kbo/Vzer subfolder pattern to the new ErkenningScenarioBuilder pattern. Use when asked to migrate a specific "When_*" erkenningen commandhandling test folder.
---

## Goal

Migrate one `When_*/CommandHandling/` folder from the old pattern (separate `Kbo/` and `Vzer/` subfolders with near-duplicate test classes that each use a dedicated named scenario class) to the new pattern (single `[Theory]` test combining both association types via `ErkenningScenarioBuilder`).

After migration:
- The `Kbo/` and `Vzer/` subfolders are **deleted**
- Combined tests live in `CommandHandling/` root (or in `VerenigingErkendStatusTests/` subfolder if they specifically test `VerenigingWerdErkend`/`VerenigingWerdNietLangerErkend` transitions)
- Vzer-only tests (no Kbo equivalent) become a single `[Fact]` in the root using the builder for the VZER type only

---

## Key files to understand before starting

- **ErkenningScenarioBuilder**: `test/AssociationRegistry.Test.Admin.Api/DecentraalBeheer/Verenigingen/Erkenningen/ErkenningScenarioBuilder.cs`
- **Completed example** (VerenigingErkendStatus): `When_Hef_Schorsing_Erkenning_Op/CommandHandling/VerenigingErkendStatusTests/Given_Actieve_Erkenning_And_Vereniging_Erkend.cs`
- **Completed example** (general test): `When_Activeer_Erkenning/CommandHandling/Given_A_Valid_Command.cs`
- **Context class example**: `When_Activeer_Erkenning/CommandHandling/ActiveerErkenningContext.cs`

---

## Step-by-step process

### 1. Read all files in the target `CommandHandling/` folder

Read every file in the folder being migrated — both in the root and in `Kbo/` and `Vzer/` subfolders. Identify:

- The **context class** in the root (e.g. `ActiveerErkenningContext.cs`)
- All test files in `Kbo/` and `Vzer/` — list them side by side
- For each filename that exists in both `Kbo/` and `Vzer/`: compare the content. They will differ only in the scenario class name (Vmr vs Vzer variant)
- Files that exist only in `Vzer/` (no Kbo equivalent): note these separately

Also check the `ErkenningScenarioBuilder` to know which builder methods are available. If a needed method is missing, you will have to add it (see step 5).

### 2. Classify each pair of test files

For each unique test name that appears in both `Kbo/` and `Vzer/`:

**Category A — VerenigingErkend status test:**
The test asserts that `VerenigingWerdErkend` or `VerenigingWerdNietLangerErkend` is saved. These go into the `VerenigingErkendStatusTests/` subfolder.

**Category B — General combined test:**
The test has nothing specific to `VerenigingWerdErkend` (e.g. throws an exception, saves a domain event unrelated to erkend-status). These go in the `CommandHandling/` root.

For each file that exists only in `Vzer/` (no Kbo equivalent):

**Category C — Vzer-only test:**
The test concerns a VZER-specific domain concept (e.g. `Given_Dubbele_Vereniging`). Keep as a single `[Fact]` in the `CommandHandling/` root, using the builder for VZER only.

### 3. Ensure the context class has the fluent API

Before writing test files, verify the context class has these members. If missing, add them — following `WijzigErkenningContext` and `ActiveerErkenningContext` as reference:

```csharp
public class XxxErkenningContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    public Fixture Fixture { get; }
    // ...
    public CommandMetadata Metadata { get; private set; }
    public XxxErkenningCommand Command { get; private set; } = null!;  // NOT set in constructor

    public XxxErkenningContext(TScenario scenario, Func<TScenario, int> defaultErkenningId,
        Func<TScenario, string>? defaultInitiator = null)
    {
        // setup Fixture, Scenario, AggregateSessionMock, _commandHandler, Metadata
        // DO NOT call CreateCommand() here — Command stays null! until WithCommand() is called
    }

    public static XxxErkenningContext<TScenario> Given(
        TScenario scenario,
        Func<TScenario, int> erkenningIdSelector,
        Func<TScenario, string>? defaultInitiator = null)
        => new(scenario, erkenningIdSelector, defaultInitiator);

    public XxxErkenningContext<TScenario> WithCommand(
        Func<XxxErkenningCommand, XxxErkenningCommand> configure)
    {
        Command = CreateCommand();       // always re-create from scratch
        Command = configure(Command);   // then apply caller's modifications
        return this;
    }

    public XxxErkenningContext<TScenario> WithInitiator(string? ovoCode = null)
    {
        Metadata = Metadata with { Initiator = ovoCode ?? Fixture.Create<string>() };
        return this;
    }

    public async ValueTask<XxxErkenningContext<TScenario>> WhenHandled()
    {
        await _commandHandler.Handle(
            new CommandEnvelope<XxxErkenningCommand>(Command, Metadata));
        return this;
    }

    public void ShouldHaveSaved(params IEvent[] events)
        => AggregateSessionMock.ShouldHaveSavedExact(events);

    private XxxErkenningCommand CreateCommand() => ...;
}
```

**Rules for the context class:**
- `Command` is declared `= null!` and is **never set in the constructor**
- `WithCommand` always calls `CreateCommand()` first, then applies the lambda
- `WithCommand` returns `this` for fluent chaining
- `WhenHandled` returns `Task<XxxErkenningContext<TScenario>>` so the caller can chain `await ... .WhenHandled()` and get the ctx back for assertions
- `ShouldHaveSaved(params IEvent[])` is a thin wrapper around `AggregateSessionMock.ShouldHaveSavedExact`
- `Fixture` is **public** so tests can call `ctx.Fixture.Create<...>()`
- The command property is named `Command`, never after the command type

### 4. Write the merged test classes

#### Template for Category A and B — merged `[Theory]` test

```csharp
namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen
    .When_XxxErkenning.CommandHandling[.VerenigingErkendStatusTests];

using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Events;
using Xunit;

public class Given_SomeScenarioName
{
    public static IEnumerable<object[]> ErkenningScenarios
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();

            var (vzerErkenningId, vzer) = new ErkenningScenarioBuilder(fixture)
                .With...()
                .BuildForVzer();

            var (vmrErkenningId, vmr) = new ErkenningScenarioBuilder(fixture)
                .With...()
                .BuildForVmr();

            return new[] { new object[] { vzer, vzerErkenningId }, new object[] { vmr, vmrErkenningId } };
        }
    }

    [Theory]
    [MemberData(nameof(ErkenningScenarios))]
    public async ValueTask Then_Saves_...(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var ctx = await XxxErkenningContext<CommandhandlerScenarioBase>
            .Given(
                scenario,
                _ => erkenningId,
                _ =>
                    scenario
                        .GetVerenigingState()
                        .Erkenningen.Single(e => e.ErkenningId == erkenningId)
                        .GeregistreerdDoor.OvoCode
            )
            .WithCommand(cmd => cmd)    // identity if no field overrides needed
            .WhenHandled();

        ctx.ShouldHaveSaved(/* ... expected events ... */);
    }
}
```

**`.WithCommand(cmd => cmd)` is always required** — even for tests that need no modifications — because `Command` is not set in the constructor.

When a test needs to override a field:
```csharp
.WithCommand(cmd =>
    cmd with
    {
        Erkenning = cmd.Erkenning with
        {
            StartDatum = NullOrEmpty<DateOnly>.Create(startdatum),
            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(hernieuwingsdatum),
            EindDatum = NullOrEmpty<DateOnly>.Create(eindDatum),
        },
    }
)
```

**Builder chain — mapping events to methods:**

| Event type in old scenario | Builder method |
|---|---|
| `ErkenningWerdGeregistreerd` with active date range | `.WithActieveErkenning()` |
| `ErkenningWerdGeregistreerd` with future start date | `.WithInAanvraagErkenning()` |
| `ErkenningWerdGeregistreerd` InAanvraag with past start (activatable today) | `.WithTeActiverenErkenning()` |
| `ErkenningWerdVerlopen` | `.WithVerlopenErkenning()` |
| `VerenigingWerdErkend` | `.WithVerenigingWerdErkend()` |
| `ErkenningWerdGeschorst` | `.WithErkenningWerdGeschorst()` |
| `VerenigingWerdNietLangerErkend` | `.WithVerenigingWerdNietLangerErkend()` |
| `ErkenningWerdGewijzigd` with future start date | `.WithGewijzigdNaarInAanvraag()` |

If a needed event is not covered, add a new `With...()` method to `ErkenningScenarioBuilder.cs`.

#### Template for Category C — Vzer-only `[Fact]`

```csharp
[Fact]
public async ValueTask Then_...()
{
    var fixture = new Fixture().CustomizeDomain();

    var (erkenningId, scenario) = new ErkenningScenarioBuilder(fixture)
        .With...()
        .BuildForVzer();

    // optional: append extra events to scenario.additionalEvents
    scenario.additionalEvents.Add(new SomeExtraEvent(...));

    var ctx = await XxxErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario>
        .Given(
            scenario,
            _ => erkenningId,
            _ =>
                scenario
                    .GetVerenigingState()
                    .Erkenningen.Single(e => e.ErkenningId == erkenningId)
                    .GeregistreerdDoor.OvoCode
        )
        .WithCommand(cmd => cmd)
        .WhenHandled();

    ctx.ShouldHaveSaved(...);
}
```

#### Tests that expect an exception

```csharp
var ctx = XxxErkenningContext<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId, _ => ...);

var exception = await Assert.ThrowsAsync<SomeDomainException>(
    async () => await ctx.WithCommand(cmd => cmd).WhenHandled());

ctx.AggregateSessionMock.ShouldNotHaveAnySaves();
exception.Message.Should().Be(...);
```

Use `Assert.ThrowsAsync<TException>` — **never** FluentAssertions `.Should().ThrowAsync<>`.

#### Tests with a different initiator

Use `.WithInitiator(ovoCode)` in the fluent chain:
```csharp
var ctx = await XxxErkenningContext<CommandhandlerScenarioBase>
    .Given(scenario, _ => erkenningId)
    .WithCommand(cmd => cmd)
    .WithInitiator(WellknownOvoNumbers.SomeBlockedCode)
    .WhenHandled();
```

### 5. Add missing builder methods if needed

```csharp
public ErkenningScenarioBuilder WithSomeNewState()
{
    Events.Add(new SomeNewEvent(_erkenningId, ...));
    return this;
}
```

### 6. Delete the `Kbo/` and `Vzer/` subfolders

```bash
rm -rf path/to/When_Xxx/CommandHandling/Kbo
rm -rf path/to/When_Xxx/CommandHandling/Vzer
```

### 7. Verify compilation and run tests

First build:

```bash
dotnet build test/AssociationRegistry.Test.Admin.Api/AssociationRegistry.Test.Admin.Api.csproj --no-restore -c Release
```

Then run **only the tests in the migrated folder** (replace `When_Xxx` with the actual folder name):

```bash
dotnet test test/AssociationRegistry.Test.Admin.Api/AssociationRegistry.Test.Admin.Api.csproj --no-build -c Release --filter "FullyQualifiedName~DecentraalBeheer.Verenigingen.Erkenningen.When_Xxx.CommandHandling"
```

All tests in the migrated folder must pass before the migration is considered complete.

---

## File placement rules

| Situation | Where to put the file |
|---|---|
| Test asserts `VerenigingWerdErkend` or `VerenigingWerdNietLangerErkend` in `ShouldHaveSaved` | `CommandHandling/VerenigingErkendStatusTests/Given_X.cs` |
| Test covers two types (Kbo + Vzer) but is not about erkend-status | `CommandHandling/Given_X.cs` (root) |
| Test is Vzer-only (no Kbo equivalent) | `CommandHandling/Given_X.cs` (root, single `[Fact]`) |
| Context class | `CommandHandling/XxxContext.cs` (root, unchanged) |

---

## What NOT to do

- **Never** set `Command` in the context constructor — it is always `null!` until `WithCommand()` is called
- **Never** omit `.WithCommand(cmd => cmd)` before `.WhenHandled()` — even for tests with no field overrides
- **Never** name the command property after the command type — always use `Command`
- **Never** reference old named scenario classes (`...WithXxxScenario`) in test files — use the builder
- **Never** create a new named scenario class — the builder replaces them
- **Never** use `[Fact]` for a test that can be expressed as a `[Theory]` covering both types
- **Never** write `with { Prop = value }` on a single line — always multiline with trailing comma
- **Never** use FluentAssertions `.Should().ThrowAsync<>` — use `Assert.ThrowsAsync<>`
- **Never** inline `new Fixture()`, `new AggregateSessionMock()`, `new CommandEnvelope<>()` in a test method
