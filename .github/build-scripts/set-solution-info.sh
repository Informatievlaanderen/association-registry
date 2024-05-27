dotnet tool restore;
dotnet tool install dotnet-script;
dotnet tool update dotnet-script;
dotnet dotnet-script .github/build-scripts/SetSolutionInfo.csx
