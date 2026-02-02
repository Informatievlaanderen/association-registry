# Code Quality Setup

This document explains how we work and the tools we use.

## How We Work

**Formatting:** Automatic on save and commit. No discussions about spacing.

**Secrets:** Detected before commit. Allow with `detect-secrets scan --baseline .secrets.baseline`.

**Tools:** Used as intended. Replaced if slow or broken.

**Three levels:**
1. IDE - Format on save
2. Pre-commit hook - Format + scan for secrets
3. CI - Final check (should rarely fail)

## Coding Guidelines

### Visual Readability

Code shape matters. Structure should be visible at a glance.

**Good - structure clear:**
```csharp
var result = collection
    .Where(x => x.IsValid)
    .Select(x => x.Value)
    .ToList();

SomeMethod(
    argument1,
    argument2);
```

**Bad - aligned to names:**
```csharp
var result = someLongMethodName(param1,
                                 param2);
```

Code should be readable when all names are replaced with "x" (Kevlin Henney).

### Where Rules Live

**`.editorconfig`** - Cross-IDE basics (indentation, line length, namespaces)

**`.DotSettings`** - Rider-specific (wrapping style, inspection severity)

## Setup

### Tools We Use

- **CSharpier** - Formats C# code (fast, no solution loading)
- **detect-secrets** - Scans for secrets with baseline approach
- **Husky** - Runs pre-commit hooks

### Requirements

- .NET 9 SDK
- Python 3 (for detect-secrets via pipx)
- Git

### Installation

**1. Run setup script:**

Linux/macOS:
```bash
./scripts/setup-dev-environment.sh
```

Windows (Git Bash):
```bash
bash scripts/setup-dev-environment.sh
```

**2. Install missing tools (if script reports them):**

CSharpier:
```bash
dotnet tool install -g csharpier
```

detect-secrets:
```bash
# Install pipx first
python3 -m pip install --user pipx  # Linux/macOS
py -m pip install --user pipx        # Windows

# Install detect-secrets
pipx install detect-secrets
py -m pip install detect-secrets # Windows
```

**3. Rider (optional):**
- Settings → Tools → Actions on Save → ✓ Reformat code

## Tools

### Husky (`.husky/`)

**What:** Git hooks manager
**Hook configured:** Pre-commit

**Pre-commit hook does:**
1. Formats staged C# files with CSharpier
2. Scans for secrets with detect-secrets
3. Fast execution (no solution loading)
4. Automatically adds formatted files back to staging

**Configuration:**
```bash
git config core.hooksPath .husky
```

**Files:**
- `.husky/pre-commit` - The actual hook script
- `.secrets.baseline` - Allowed secrets baseline

### CSharpier

Opinionated C# formatter. Fast - formats individual files without loading entire solution.

**Installation:**
```bash
dotnet tool install -g csharpier
```

### detect-secrets

Secret scanner with baseline approach.

**Baseline:**
- Current secrets stored in `.secrets.baseline` (hashed)
- Only NEW secrets block commits
- Modifying existing secret = detected as new

**When secret detected:**
- Remove it (use env vars or secret manager)
- OR allow it: `detect-secrets scan --baseline .secrets.baseline`

**Installation:**
```bash
pipx install detect-secrets
```

## Troubleshooting

### "detect-secrets not found"

Install via pipx:
```bash
pipx install detect-secrets
```

### "csharpier not found"

Install globally:
```bash
dotnet tool install -g csharpier
```

### Hook not running

Check hooks are executable and path is set:
```bash
chmod +x .husky/pre-commit
git config core.hooksPath .husky
```

### Bypass hooks (emergency only)

```bash
git commit --no-verify -m "message"
```

**Warning:** Skips formatting and security checks!

## Maintenance

### Updating formatting rules

1. Update `.editorconfig` and/or `.DotSettings`
2. Run `dotnet format` on entire solution
3. Review changes carefully
4. Commit with: `style: update code formatting rules`
5. Document changes in this file

### Updating CSharpier

```bash
dotnet tool update -g csharpier
```

### Updating detect-secrets

```bash
pipx upgrade detect-secrets
```

## Relevant Files

- ✅ `.editorconfig` - Enhanced with C# rules
- ✅ `.DotSettings` - Updated Rider settings
- ✅ `.husky/pre-commit` - Git pre-commit hook
- ✅ `.secrets.baseline` - Allowed secrets baseline
- ✅ `.gitleaks.toml` - Gitleaks config (legacy, replaced by detect-secrets)
- ✅ `.github/workflows/cicd.pull-request.yml` - Added format check
- ✅ `scripts/setup-dev-environment.sh` - New setup script
- ✅ `CODE_QUALITY_SETUP.md` - This documentation

## References

- [EditorConfig Documentation](https://editorconfig.org/)
- [CSharpier](https://csharpier.com/)
- [detect-secrets GitHub](https://github.com/Yelp/detect-secrets)
- [Husky GitHub](https://typicode.github.io/husky/)
- [dotnet format Documentation](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-format)
- [Kevlin Henney - Seven Ineffective Coding Habits](https://www.youtube.com/watch?v=ZsHMHukIlJY)