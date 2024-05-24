#r "paket:
version 8.0.3
framework: auto-detect
source https://api.nuget.org/v3/index.json
nuget Be.Vlaanderen.Basisregisters.Build.Pipeline //"

#load "packages/Be.Vlaanderen.Basisregisters.Build.Pipeline/Content/build-generic.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.IO.FileSystemOperators
open Fake.DotNet
open System
open ``Build-generic``



let product = "Basisregisters Vlaanderen"
let copyright = "Copyright (c) Vlaamse overheid"
let company = "Vlaamse overheid"

let assemblyVersionNumber = (sprintf "%s.0")
let nugetVersionNumber = (sprintf "%s")

let buildSolution = buildSolution assemblyVersionNumber
let buildSource = build assemblyVersionNumber
let buildTest = buildTest assemblyVersionNumber
let setVersions = (setSolutionVersions assemblyVersionNumber product copyright company)
let test = testSolution
let publishSource = publish assemblyVersionNumber
let pack = pack nugetVersionNumber

let containerize project containerName =
  let result1 =
    [ "build"; "."; "--no-cache"; "--tag"; sprintf "%s/%s:%s" dockerRegistry containerName buildNumber; "--build-arg"; sprintf "build_number=%s" buildNumber]
    |> CreateProcess.fromRawCommand "docker"
    |> CreateProcess.withWorkingDirectory (buildDir @@ project @@ "linux")
    |> CreateProcess.withTimeout (TimeSpan.FromMinutes 5.)
    |> Proc.run

  if result1.ExitCode <> 0 then failwith "Failed result from Docker Build"

  let result2 =
    [ "tag"; sprintf "%s/%s:%s" dockerRegistry containerName buildNumber; sprintf "%s/%s:latest" dockerRegistry containerName]
    |> CreateProcess.fromRawCommand "docker"
    |> CreateProcess.withTimeout (TimeSpan.FromMinutes 5.)
    |> Proc.run

  if result2.ExitCode <> 0 then failwith "Failed result from Docker Tag"

let push containerName =
  let result1 =
    [ "push"; sprintf "%s/%s:%s" dockerRegistry containerName buildNumber]
    |> CreateProcess.fromRawCommand "docker"
    |> CreateProcess.withTimeout (TimeSpan.FromMinutes 5.)
    |> Proc.run

  if result1.ExitCode <> 0 then failwith "Failed result from Docker Push"

  let result2 =
    [ "push"; sprintf "%s/%s:latest" dockerRegistry containerName]
    |> CreateProcess.fromRawCommand "docker"
    |> CreateProcess.withTimeout (TimeSpan.FromMinutes 5.)
    |> Proc.run

  if result2.ExitCode <> 0 then failwith "Failed result from Docker Push Latest"

supportedRuntimeIdentifiers <- [ "msil"; "linux-x64" ]

let testWithDotNet path =
  let fxVersion = getDotNetClrVersionFromGlobalJson()

  let cmd = sprintf "test --no-build --no-restore --logger trx --configuration Release --no-build --no-restore /p:RuntimeFrameworkVersion=%s --dcReportType=HTML" fxVersion

  let result = DotNet.exec (id) "dotcover" cmd
  if result.ExitCode <> 0 then failwith "Test Failure"

let testSolution sln =
  testWithDotNet (sprintf "%s.sln" sln)

// Solution -----------------------------------------------------------------------
Target.create "SetAssemblyVersions" (fun _ -> setVersions "SolutionInfo.cs")

Target.create "Restore_Solution" (fun _ -> restore "AssociationRegistry")

Target.create "Build_Solution" (fun _ ->
  setVersions "SolutionInfo.cs"
  buildSolution "AssociationRegistry")

Target.create "SetSolutionInfo" (fun _ -> setVersions "SolutionInfo.cs")

Target.create "Test_Solution" (fun _ -> testSolution "AssociationRegistry")

Target.create "Pack_Solution" (fun _ ->
  [
    "AssociationRegistry"
    "AssociationRegistry.Magda"
  ] |> List.iter pack
)

Target.create "Publish_Solution" (fun _ ->
  [
    "AssociationRegistry.Acm.Api"
    "AssociationRegistry.Public.Api"
    "AssociationRegistry.Public.ProjectionHost"
    "AssociationRegistry.Admin.Api"
    "AssociationRegistry.Admin.ProjectionHost"
  ] |> List.iter publishSource)

Target.create "Containerize_AcmApi" (fun _ -> containerize "AssociationRegistry.Acm.Api" "verenigingsregister-acmapi")
Target.create "PushContainer_AcmApi" (fun _ -> push "verenigingsregister-acmapi")

Target.create "Containerize_PublicApi" (fun _ -> containerize "AssociationRegistry.Public.Api" "verenigingsregister-publicapi")
Target.create "PushContainer_PublicApi" (fun _ -> push "verenigingsregister-publicapi")

Target.create "Containerize_PublicProjections" (fun _ -> containerize "AssociationRegistry.Public.ProjectionHost" "verenigingsregister-publicprojections")
Target.create "PushContainer_PublicProjections" (fun _ -> push "verenigingsregister-publicprojections")

Target.create "Containerize_AdminApi" (fun _ -> containerize "AssociationRegistry.Admin.Api" "verenigingsregister-adminapi")
Target.create "PushContainer_AdminApi" (fun _ -> push "verenigingsregister-adminapi")

Target.create "Containerize_AdminProjections" (fun _ -> containerize "AssociationRegistry.Admin.ProjectionHost" "verenigingsregister-adminprojections")
Target.create "PushContainer_AdminProjections" (fun _ -> push "verenigingsregister-adminprojections")

// --------------------------------------------------------------------------------

Target.create "Build" ignore
Target.create "Test" ignore
Target.create "Publish" ignore
Target.create "Pack" ignore
Target.create "Containerize" ignore
Target.create "Push" ignore

"NpmInstall"
  ==> "DotNetCli"
  ==> "Clean"
  ==> "Restore_Solution"
  ==> "Build_Solution"
  ==> "Build"

"Build"
  ==> "Test_Solution"
  ==> "Test"

"Test"
  ==> "Publish_Solution"
  ==> "Publish"

"Publish"
  ==> "Pack"

"Containerize"
  ==> "DockerLogin"
  ==> "PushContainer_AcmApi"
  ==> "PushContainer_PublicApi"
  ==> "PushContainer_PublicProjections"
  ==> "PushContainer_AdminApi"
  ==> "PushContainer_AdminProjections"
  ==> "Push"
// Possibly add more projects to push here

// By default we build & test
Target.runOrDefault "Test"
