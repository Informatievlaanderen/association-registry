#!/bin/sh

rm -rfv ./**/*/Internal/
dotnet run --project src/AssociationRegistry.Admin.ProjectionHost -- codegen write
dotnet run --project src/AssociationRegistry.Admin.Api -- codegen write
dotnet run --project src/AssociationRegistry.Public.ProjectionHost -- codegen write
dotnet run --project src/AssociationRegistry.Acm.Api -- codegen write
pushd src/AssociationRegistry.Admin.AddressSync
dotnet run -- codegen write
popd
