#!/bin/bash

# Setup script for association-registry development environment
# Works on Linux, macOS, and Windows (Git Bash/WSL)

set -e

echo "Setting up development environment..."
echo ""

# Check .NET SDK
if ! command -v dotnet &> /dev/null; then
    echo "ERROR: .NET SDK not found!"
    echo "Install .NET 9 SDK from: https://dotnet.microsoft.com/download"
    exit 1
fi
echo "✓ .NET SDK: $(dotnet --version)"

# Configure git hooks path
echo ""
echo "Configuring git hooks..."
git config core.hooksPath .husky
chmod +x .husky/pre-commit
echo "✓ Git hooks configured"

# Check CSharpier
echo ""
if command -v csharpier &> /dev/null; then
    echo "✓ CSharpier found"
else
    echo "⚠ CSharpier not installed"
    echo ""
    echo "Install: dotnet tool install -g csharpier"
fi

# Check detect-secrets
echo ""
if command -v detect-secrets &> /dev/null; then
    echo "✓ detect-secrets found"
else
    echo "⚠ detect-secrets not installed"
    echo ""
    if command -v pipx &> /dev/null; then
        echo "Install: pipx install detect-secrets"
    else
        echo "Install pipx first, then: pipx install detect-secrets"
        echo ""
        echo "  Linux/macOS: python3 -m pip install --user pipx"
        echo "  Windows:     py -m pip install --user pipx"
    fi
fi

# Rider reminder
echo ""
echo "Rider setup (optional):"
echo "  Settings → Tools → Actions on Save"
echo "  ✓ Reformat code"

echo ""
echo "✓ Setup complete!"
echo ""
echo "Install missing tools, then test with a commit."
echo ""
