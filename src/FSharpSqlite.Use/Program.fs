module FsharpSqlite.Use.Program


type Convert = System.Convert

module internal Text =
    type Encoding = System.Text.Encoding
    
module internal Crypto =
    open System.Security
    type RNGCryptoServiceProvider = Cryptography.RNGCryptoServiceProvider
    type SHA256 = Cryptography.SHA256
    
type Sql = FsharpSqlite.Use.SqlProvider.Sql

type Persistence() =
    let dc = Sql.GetDataContext()
        
    member __.findOrCreateUser
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
            let rngProvider = new Crypto.RNGCryptoServiceProvider()

            let saltBytes = Array.create 10 (new byte())
            rngProvider.GetBytes(saltBytes)
            user.PasswordSalt <- Convert.ToBase64String(saltBytes)
            let passwordSaltBytes = Array.append <| Text.Encoding.UTF8.GetBytes(password) <| saltBytes
            user.PasswordHash <- Convert.ToBase64String(Crypto.SHA256.Create().ComputeHash(passwordSaltBytes))
            dc.SubmitUpdates()
            user
        | Some user -> user


let WorkWithDb =
    let p = new Persistence()
    let john = p.findOrCreateUser "John" "Lenon" "john@beatles.com" "HeyJude123"
    printfn "John's id is %d" john.Id
    ()

[<EntryPoint>]
let main argv =
    let ret = 0
    WorkWithDb
    0 // return an integer exit code
