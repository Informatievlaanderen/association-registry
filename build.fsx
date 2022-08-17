#r "paket:
version 7.0.2
framework: net6.0
source https://api.nuget.org/v3/index.json
nuget Be.Vlaanderen.Basisregisters.Build.Pipeline 6.0.3 //"

#load "packages/Be.Vlaanderen.Basisregisters.Build.Pipeline/Content/build-generic.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.IO.FileSystemOperators
open ``Build-generic``

let product = "Basisregisters Vlaanderen"
let copyright = "Copyright (c) Vlaamse overheid"
let company = "Vlaamse overheid"

let dockerRepository = "association-registry"
let assemblyVersionNumber = (sprintf "2.%s")
let nugetVersionNumber = (sprintf "%s")

let buildSolution = buildSolution assemblyVersionNumber
let buildSource = build assemblyVersionNumber
let buildTest = buildTest assemblyVersionNumber
let setVersions = (setSolutionVersions assemblyVersionNumber product copyright company)
let test = testSolution
let publishSource = publish assemblyVersionNumber
let pack = pack nugetVersionNumber
let containerize = containerize dockerRepository
let push = push dockerRepository

supportedRuntimeIdentifiers <- [ "msil"; "linux-x64" ]

// Solution -----------------------------------------------------------------------

Target.create "Restore_Solution" (fun _ -> restore "AssociationRegistry")

Target.create "Build_Solution" (fun _ ->
  setVersions "SolutionInfo.cs"
  buildSolution "AssociationRegistry")

Target.create "Test_Solution" (fun _ -> test "AssociationRegistry")

Target.create "Publish_Solution" (fun _ ->
  [
    "AssociationRegistry.Acm.Api"
  ] |> List.iter publishSource)

Target.create "Pack_Solution" (fun _ ->
  [
    "AssociationRegistry.Acm.Api"
  ] |> List.iter pack)

Target.create "Containerize_AcmApi" (fun _ -> containerize "AssociationRegistry.Acm.Api" "acm-api")
Target.create "PushContainer_AcmApi" (fun _ -> push "acm-api")

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
  ==> "Pack_Solution"
  ==> "Pack"

"Pack"
  ==> "Containerize_AcmApi"
  ==> "Containerize"
// Possibly add more projects to containerize here

"Containerize"
  ==> "DockerLogin"
  ==> "PushContainer_AcmApi"
  ==> "Push"
// Possibly add more projects to push here

// By default we build & test
Target.runOrDefault "Test"
