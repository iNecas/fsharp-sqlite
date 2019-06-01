module FSharpSqlite.Migrate.Program

open FluentMigrator.Runner
open Microsoft.Extensions.DependencyInjection

open System.IO

let sqliteConString = "Data Source=" + Path.Join(__SOURCE_DIRECTORY__, "../FSharpSqlite.Use/provider.db")

let CreateServices(conString: string) =
    let sc = new ServiceCollection()
    sc.AddFluentMigratorCore()
      .ConfigureRunner(fun rb ->
              rb.AddSQLite().WithGlobalConnectionString(conString)
                .ScanIn(System.Reflection.Assembly.GetExecutingAssembly()).For.Migrations()
              |> ignore
          )
      .AddLogging(fun lb -> lb.AddFluentMigratorConsole() |> ignore)
      .BuildServiceProvider(false)

let MigrateDatabase (conString: string) =
    printfn "Starting migration"
    let serviceProvider = CreateServices(conString)
    use scope = serviceProvider.CreateScope()
    let runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>()
    runner.MigrateUp()
    printfn "Migration finished"

[<EntryPoint>]
let main argv =
    let ret = 0
    printfn "con string is %s" sqliteConString
    MigrateDatabase(sqliteConString)
    0
