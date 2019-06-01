module FsharpSqlite.Use.Program

open Microsoft.Extensions.DependencyInjection
open FsharpSqlite.Use.SqlProvider

open System
open System.Security.Cryptography
open System.Text

let findOrCreateUser
    (dc : Sql.dataContext)
    (firstName: string) (lastName: string) (email: string) (password: string) : Sql.dataContext.``main.UsersEntity`` =
    let userQuery = query {
        for user in dc.Main.Users do
            where (user.Email = email)
            take 1
            select (user)
    }
    let maybeUser = userQuery |> Seq.tryHead
    match maybeUser with
    | None ->
        let user =  dc.Main.Users.Create()
        user.FirstName <- firstName
        user.LastName <- lastName
        user.Email <- email
        let rngProvider = new RNGCryptoServiceProvider()

        let saltBytes = Array.create 10 (new byte())
        rngProvider.GetBytes(saltBytes)
        user.PasswordSalt <- Convert.ToBase64String(saltBytes)
        let passwordSaltBytes = Array.append <| Encoding.UTF8.GetBytes(password) <| saltBytes
        user.PasswordHash <- Convert.ToBase64String(SHA256.Create().ComputeHash(passwordSaltBytes))
        dc.SubmitUpdates()
        user
    | Some john -> john


let WorkWithDb =
    let dc = Sql.GetDataContext()
    let john = findOrCreateUser dc "John" "Lenon" "john@beatles.com" "HeyJude123"
    printfn "John's id is %d" john.Id
    ()

[<EntryPoint>]
let main argv =
    let ret = 0
    WorkWithDb
    0 // return an integer exit code
