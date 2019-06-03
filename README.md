# FSharpSqlite

An example project using F# and SQLite, that involves:

- defining and running DB in F# using [fluentmigrator](https://github.com/fluentmigrator/fluentmigrator)
- setup of [SqlProvider](https://fsprojects.github.io/SQLProvider/) with SQLite (involving copying the files to
right place and custom build for additional hotfix for [a bug](https://github.com/fsprojects/SQLProvider/pull/618).
)
- using SqlProvider to work with the data
- running on top of dotnet core v3.0 using Linux containers

## Usage

The simplest way to get the reproducible setup is to use Linux containers. This instructions
prefer [Buildah](https://github.com/containers/buildah) and [Podman](https://podman.io/) over
`docker`, as they don't need any additional daemon to be running on the system. Corresponding
`docker` commands should still work though.

## Building app container

To build the sample application image, run:

```
buildah bud -t inecas/fsharp-sqlite .
```

This will download the [dotnet core v3 SDK image](https://hub.docker.com/_/microsoft-dotnet-core-sdk/)
and will prepare the setup for running the application.

## Reproducing SQLite issue

This repository is also used as a reproducer for an issue `SqlProvider` has with `SQLite`
[described here](https://github.com/fsprojects/SQLProvider/pull/618).

In order reproduce the issue just run:

```
podman run --rm -ti --name inecas-fsharp-sqlite inecas/fsharp-sqlite
# within the container shell:
dotnet build src/FSharpSqlite.Use
# The command fails with
/app/src/FSharpSqlite.Use/Persistence.fs(14,16): error FS3033: The type provider 'FSharp.Data.Sql.SqlTypeProvider' reported an error: Not supported. This custom getSchema will be removed when the corresponding System.Data.Common interface is supported by the connection driver. [/app/src/FSharpSqlite.Use/FSharpSqlite.Use.fsproj]
```

## Fix for the SQLite issue

There is a custom build of `SqlProvider` that incorporates the fix. To use it:

```
podman run --rm -ti --name inecas-fsharp-sqlite inecas/fsharp-sqlite
cp hotfix/FSharp.Data.SqlProvider.dll ~/.nuget/packages/sqlprovider/*/lib/netstandard2.0/
dotnet run -p src/FSharpSqlite.Use
# outputs: John's id is 1
```
