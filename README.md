# FSharpSqlite

An example project using F# and SQLite, that involves:

- defining and running DB in F# using [fluentmigrator](https://github.com/fluentmigrator/fluentmigrator)
- setup of [SqlProvider](https://fsprojects.github.io/SQLProvider/) with SQLite (involving copying the files to
right place)
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


## Usage

```
podman run --rm -ti --name inecas-fsharp-sqlite inecas/fsharp-sqlite
dotnet run -p src/FSharpSqlite.Use
# outputs: John's id is 1
```
