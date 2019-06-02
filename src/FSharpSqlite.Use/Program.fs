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

type PersistedUser = {
    Id: int64
    User: User
}

type Persistence() =
    let dc = Sql.GetDataContext()
    
    member this.CreateUser (user: User) : PersistedUser =
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
        { Id = u.Id; User = { user with Password = PasswordHash passwordHash } }
     
     member this.FindUserByEmail (email: string) : PersistedUser option =
         let userQuery = query {
            for u in dc.Main.Users do
                where (u.Email = email)
                take 1
                select (u)
         }
         let recordToUser (record: Sql.dataContext.``main.UsersEntity``) =
             let user = { FirstName = record.FirstName
                          LastName = record.LastName
                          Email = record.Email
                          Password = PasswordHash { Hash = record.PasswordHash; Salt = record.PasswordSalt } }
             { Id = record.Id; User = user }
         userQuery |> Seq.tryHead |> Option.map recordToUser
         
        
    member this.FindOrCreateUser (user: User) : PersistedUser =
        match this.FindUserByEmail(user.Email) with
        | None -> this.CreateUser user
        | Some u -> u


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
