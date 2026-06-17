---
name: rewrite-command-handler-tests
description: Refactor command handler tests under a CommandHandling/ folder by extracting repeated boilerplate (fixture, scenario, mock, handler) into a composition-based context class. Use this when asked to refactor or rewrite existing command handler tests.
---

## Goal

Every `Given_*` test class in a `CommandHandling/` folder currently repeats the same four private fields and the same constructor setup. Extract that shared setup into a single **generic** `*Context<TScenario>` class that each `Given_*` class uses via a single private field. No base classes, no inheritance, no per-scenario context subclasses.

The scenario is **passed into the context by the test class**, together with a lambda that selects the default value for the command's scenario-dependent field. This makes the test immediately readable: you see both the scenario and the relevant property on the same line as the context.

---

## Step-by-step process

### 1. Read all `Given_*` files in the target folder

Before writing anything, read every `Given_*` file. Identify:
- The **command handler type** (e.g. `ValideerBankrekeningnummerCommandHandler`)
- The **command type** (e.g. `ValideerBankrekeningnummerCommand`)
- All **scenario types** used across the `Given_*` classes
- The **mock type** — usually `AggregateSessionMock`
- Which fields each test overrides when building a command or metadata
- The **scenario-dependent field** in `CreateCommand` — the one whose default value comes from a scenario property that differs per scenario type (e.g. `BankrekeningnummerId` sourced from different event properties depending on the scenario)

### 2. Identify the common scenario base type

All scenarios must share a common base class or interface that provides `VCode` and `GetVerenigingState()`. Use it as the generic constraint. In this codebase the constraint is `CommandhandlerScenarioBase`.

### 3. Create one generic context class

**One file, one class** — regardless of how many scenario types are used across the `Given_*` classes.

**File placement:** The context file goes in the **root of the `Commandhandling/` folder** — never in subfolders.

**File name:** `{CommandName}Context.cs` (no scenario suffix).

**The constructor takes:**
1. The scenario instance (`TScenario scenario`)
2. One `Func<TScenario, TField>` delegate per scenario-dependent field in `CreateCommand` — this is how each test tells the context which property to use as the default

**Template:**

```csharp
public class ValideerBankrekeningnummerContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    private readonly Fixture _fixture;
    private readonly Func<TScenario, int> _defaultBankrekeningnummerId;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; }

    private readonly ValideerBankrekeningnummerCommandHandler _commandHandler;

    public ValideerBankrekeningnummerContext(TScenario scenario, Func<TScenario, int> defaultBankrekeningnummerId)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        _defaultBankrekeningnummerId = defaultBankrekeningnummerId;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState());
        _commandHandler = new ValideerBankrekeningnummerCommandHandler(AggregateSessionMock);
        Metadata = _fixture.Create<CommandMetadata>();
    }

    public ValideerBankrekeningnummerCommand CreateCommand(int? bankrekeningnummerId = null)
        => _fixture.Create<ValideerBankrekeningnummerCommand>() with
        {
            VCode = Scenario.VCode,
            BankrekeningnummerId = bankrekeningnummerId ?? _defaultBankrekeningnummerId(Scenario),
        };

    public int CreateUnknownBankrekeningnummerId()
        => _defaultBankrekeningnummerId(Scenario) + _fixture.Create<int>();

    public CommandMetadata CreateMetadata(string? initiator = null)
        => _fixture.Create<CommandMetadata>() with
        {
            Initiator = initiator ?? _fixture.Create<string>(),
        };
    public async ValueTask Handle(ValideerBankrekeningnummerCommand command, CommandMetadata? metadata = null)
        => await _commandHandler.Handle(
            new CommandEnvelope<ValideerBankrekeningnummerCommand>(command, metadata ?? Metadata));
}
```

**Rules for the context class:**
- Name: `{CommandName}Context<TScenario>` — one class, no scenario suffix
- Generic constraint: the shared base class of all scenarios (e.g. `CommandhandlerScenarioBase`)
- Constructor takes the scenario **and** one `Func<TScenario, TField>` per scenario-dependent field in `CreateCommand`
- `Scenario`, `AggregateSessionMock` (or `VerenigingRepositoryMock` — use whatever the existing code calls it), and `Metadata` are **public** properties
- `_commandHandler` and all delegates are **private**
- `Metadata` is a fixture-generated `CommandMetadata` created in the constructor — use `_ctx.Metadata` in every test by default
- `CreateCommand(...)` has one optional parameter per field a test overrides; defaults use the stored delegate(s)
- `CreateMetadata(string? initiator = null)` exists only for override cases (e.g. a blocked initiator); never call it when `_ctx.Metadata` suffices
- `Handle(command, metadata?)` wraps `new CommandEnvelope<TCommand>(command, metadata ?? Metadata)` — `metadata` is optional and defaults to `Metadata`; never expose `CommandEnvelope` construction in a test method

### 4. Rewrite each `Given_*` class

Replace the boilerplate with a single field. Pass the scenario and the property selector inline:

```csharp
private readonly ValideerBankrekeningnummerContext<BankrekeningnummerWerdToegevoegdScenario> _ctx =
    new(new BankrekeningnummerWerdToegevoegdScenario(),
        s => s.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId);
```

For a KBO scenario:

```csharp
private readonly ValideerBankrekeningnummerContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario> _ctx =
    new(new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario(),
        s => s.BankrekeningnummerWerdToegevoegdVanuitKBO1.BankrekeningnummerId);
```

Then update each test method:

**Before:**
```csharp
public class Given_A_Valid_Bankrekeningnummer
{
    private readonly ValideerBankrekeningnummerCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly BankrekeningnummerWerdToegevoegdScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_A_Valid_Bankrekeningnummer()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _scenario = new BankrekeningnummerWerdToegevoegdScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());
        _commandHandler = new ValideerBankrekeningnummerCommandHandler(_aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_It_Saves_A_BankrekeningnummerWerdGewijzigd_Event()
    {
        var command = _fixture.Create<ValideerBankrekeningnummerCommand>() with
        {
            VCode = _scenario.VCode,
            BankrekeningnummerId = _scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
        };

        var commandMetadata = _fixture.Create<CommandMetadata>();

        var commandEnvelope = new CommandEnvelope<ValideerBankrekeningnummerCommand>(command, commandMetadata);

        await _commandHandler.Handle(commandEnvelope);

        _aggregateSessionMock.ShouldHaveSavedExact(
            new AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd(
                _scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                commandMetadata.Initiator
            )
        );
    }
}
```

**After:**
```csharp
public class Given_A_Valid_Bankrekeningnummer
{
    private readonly ValideerBankrekeningnummerContext<BankrekeningnummerWerdToegevoegdScenario> _ctx =
        new(new BankrekeningnummerWerdToegevoegdScenario(),
            s => s.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId);

    [Fact]
    public async ValueTask Then_It_Saves_A_BankrekeningnummerWerdGewijzigd_Event()
    {
        var command = _ctx.CreateCommand();

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd(
                _ctx.Scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                _ctx.Metadata.Initiator
            )
        );
    }
}
```

For tests that override a field (e.g. unknown id, different initiator):

```csharp
// unknown bankrekeningnummer id
var command = _ctx.CreateCommand(bankrekeningnummerId: _ctx.CreateUnknownBankrekeningnummerId());

// blocked initiator
var metadata = _ctx.CreateMetadata(initiator: WellknownOvoNumbers.VloOvoCode);
```

**Regel voor metadata in tests:** `Handle(command)` gebruikt standaard `_ctx.Metadata` — geen extra argument nodig. Geef alleen een metadata-argument mee wanneer die specifieke test een bekende waarde verwacht (bv. een geblokkeerde initiator of een initiator die terugkomt in een assertion):

```csharp
// default: geen metadata-argument
await _ctx.Handle(command);

// override: alleen wanneer de test een specifieke initiator-waarde verwacht
var metadata = _ctx.CreateMetadata(initiator: WellknownOvoNumbers.VloOvoCode);
await _ctx.Handle(command, metadata);
```

For tests that override a field that `CreateCommand` does not currently support, add an optional parameter to `CreateCommand` in the context class.

### 5. Clean up usings in every rewritten `Given_*` file

After rewriting a `Given_*` file, strip every using that was only needed for the old boilerplate. The test class no longer owns the fixture, the mock, the handler, or the command envelope — the context does.

**Always remove these usings from rewritten test files:**
- `AutoFixture` / `global::AutoFixture` — no direct `_fixture` use
- `AssociationRegistry.Framework` — no direct `CommandMetadata` or `CommandEnvelope` construction
- `AssociationRegistry.Test.Common.AutoFixture` (or the relative alias `Common.AutoFixture`) — no direct `CustomizeAdminApi()`
- `AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories` (or the relative alias `Common.StubsMocksFakes.VerenigingsRepositories`) — no direct `AggregateSessionMock`
- Any `CommandHandling.DecentraalBeheer.*` namespace that was only needed for the command handler type or to construct the command inline — `CreateCommand()` returns the command without requiring the test to name its type

**Keep only what the test method bodies and the `_ctx` field declaration directly reference:**
- The scenario type's namespace (needed for the generic type argument in the field declaration and for `_ctx.Scenario.*` property access)
- Namespaces for event types used in `ShouldHaveSavedExact(...)` assertions
- Namespaces for exception types used in `Assert.ThrowsAsync<...>`
- `FluentAssertions` — if `.Should()` is called
- `Resources` — if `ExceptionMessages.*` is used (never `AssociationRegistry.Resources`)
- `CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.ValideerBankrekening` — only if `WellknownOvoNumbers` is referenced from there
- `Xunit`

**Use relative (shortened) usings — never prefix with `AssociationRegistry.` where redundant:**
C# resolves `using` directives by walking up the enclosing namespace chain. In this project the following prefixes can be dropped:
- `using Events;` — not `using AssociationRegistry.Events;`
- `using Resources;` — not `using AssociationRegistry.Resources;`
- `using Common.Scenarios.CommandHandling.X;` — not `using AssociationRegistry.Test.Common.Scenarios.CommandHandling.X;`
- `using Common.StubsMocksFakes.VerenigingsRepositories;` — not `using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;`

**Do NOT shorten `AssociationRegistry.DecentraalBeheer.*`** — the test project already has an `AssociationRegistry.Test.Admin.Api.DecentraalBeheer` namespace, so the compiler resolves `DecentraalBeheer.*` to the wrong namespace. Always write the full `using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;`.

**Example — before cleanup:**
```csharp
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.MaakValidatieBankrekeningOngedaan;
using Events;
using Xunit;
```

**Example — after cleanup:**
```csharp
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;
```

### 6. Delete old per-scenario context files

After rewriting all `Given_*` files, delete any `*VzerContext.cs`, `*KboContext.cs`, `*GevalideerdContext.cs`, or other per-scenario context files that have been replaced by the single generic context.

### 7. Verify

After rewriting all files, confirm the test project still compiles. Do not run tests — just check that no compile errors remain.

---

## What NOT to do

- Never keep the four individual private fields (`_fixture`, `_scenario`, `_aggregateSessionMock`, `_commandHandler`) on a test class
- Never inline `new Fixture().CustomizeAdminApi()` or `new AggregateSessionMock(...)` on a test class
- Never inline `new CommandEnvelope<...>(...)` inside a test method
- Never create a base class; use a plain non-abstract context class
- **Never create multiple context classes for different scenarios** — one generic context class per command
- **Never let the context class create the scenario itself** — the scenario is always passed in by the test class
- **Never hardcode the default scenario-dependent field value inside the context** — always use the stored `Func<TScenario, TField>` delegate
