module FsharpSqlite.Use.Program

// ALIASES [[[

type Convert = System.Convert

module internal Text =
    type Encoding = System.Text.Encoding
    
module internal Crypto =
    open System.Security
    type RNGCryptoServiceProvider = Cryptography.RNGCryptoServiceProvider
    type SHA256 = Cryptography.SHA256
    
type Sql = FsharpSqlite.Use.SqlProvider.Sql

// ]]] ALIASES

type PasswordHash = {
    Hash: string
    Salt: string
}
    
type Password =
    | PlainTextPassword of string
    | PasswordHash of PasswordHash

type PasswordHash with
    static member OfPlainTextPassword (password: string) =
        let rngProvider = new Crypto.RNGCryptoServiceProvider()

        let saltBytes = Array.create 10 (new byte())
        rngProvider.GetBytes(saltBytes)
        let salt = Convert.ToBase64String(saltBytes)
        let passwordSaltBytes = Array.append <| Text.Encoding.UTF8.GetBytes(password) <| saltBytes
        let hash = Convert.ToBase64String(Crypto.SHA256.Create().ComputeHash(passwordSaltBytes))
        { Hash = hash; Salt = salt }


type User = {
    FirstName: string
    LastName: string
    Email: string
    Password: Password
}

type Persistence() =
    let dc = Sql.GetDataContext()
        
    member __.findOrCreateUser (user: User) : Sql.dataContext.``main.UsersEntity`` =
        let userQuery = query {
            for u in dc.Main.Users do
                where (u.Email = user.Email)
                take 1
                select (u)
        }
        let maybeUser = userQuery |> Seq.tryHead
        match maybeUser with
        | None ->
            let u =  dc.Main.Users.Create()
            u.FirstName <- user.FirstName
            u.LastName <- user.LastName
            u.Email <- user.Email
            let passwordHash =
                match user.Password with
                | PlainTextPassword password ->  PasswordHash.OfPlainTextPassword password
                | PasswordHash passwordHash -> passwordHash
            u.PasswordHash <- passwordHash.Hash
            u.PasswordSalt <- passwordHash.Salt
                
            dc.SubmitUpdates()
            u
        | Some u -> u


let WorkWithDb =
    let p = new Persistence()
    let john = { FirstName = "John"; LastName = "Lenon"; Email = "john@beatles.com"; Password = PlainTextPassword("HeyJude123") }
    let johnRecord = p.findOrCreateUser john
    printfn "John's id is %d" johnRecord.Id
    ()

[<EntryPoint>]
let main argv =
    let ret = 0
    WorkWithDb
    0 // return an integer exit code
