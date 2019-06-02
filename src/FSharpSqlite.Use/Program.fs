module FSharpSqlite.Use.Program

open FSharpSqlite.Use.Domain
open FSharpSqlite.Use.Persistence

let WorkWithDb =
    let p = new Persistence()
    let john = { FirstName = "John"; LastName = "Lenon"; Email = "john@beatles.com"; Password = PlainTextPassword("HeyJude123") }
    let persistedJohn = p.FindOrCreateUser john
    printfn "John's id is %d" persistedJohn.Id
    ()

[<EntryPoint>]
let main argv =
    let ret = 0
    WorkWithDb
    ret
