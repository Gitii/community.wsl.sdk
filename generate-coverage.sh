#!/usr/bin/env bash

set -e

dotnet test /p:TargetFramework=net6.0 /p:CollectCoverage=true /p:Exclude=\"[Community.Wsl.Sdk.Tests]*\" /p:CoverletOutput=../CoverageResults/ /p:MergeWith="../CoverageResults/coverage.net6.0.json" /p:CoverletOutputFormat=\"cobertura,json\" /p:ThresholdType=\"line,branch,method\" -m:1

