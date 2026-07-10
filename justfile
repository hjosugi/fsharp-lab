set shell := ["bash", "-cu"]

default:
    @just --list

restore:
    dotnet restore FSharpLab.slnx

build: restore
    dotnet build FSharpLab.slnx --no-restore

test: build
    dotnet run --project tests/FSharpLab.Tests/FSharpLab.Tests.fsproj --no-build

run: build
    dotnet run --project src/FSharpLab.Cli/FSharpLab.Cli.fsproj --no-build

basics:
    dotnet fsi labs/00-fsharp-basics.fsx

parking-solution:
    dotnet fsi labs/parking/Solution.fsx

check: test
    @echo "All checks passed."

