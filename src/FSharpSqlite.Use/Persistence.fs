module FSharpSqlite.Use.Persistence

open FSharpSqlite.Use.Domain

[<Literal>]
let resolutionPath = __SOURCE_DIRECTORY__ + "/ProviderPackages"

[<Literal>]
let providerConString = "Data Source=" + __SOURCE_DIRECTORY__ + "/data.db;"


module internal ProviderDefinition =
    open FSharp.Data.Sql
    type Sql = SqlDataProvider<Common.DatabaseProviderTypes.SQLITE,
                               providerConString,
                               ResolutionPath = resolutionPath,
                               SQLiteLibrary=Common.SQLiteLibrary.MicrosoftDataSqlite,
                               CaseSensitivityChange = Common.CaseSensitivityChange.ORIGINAL>
                              
type PersistedUser = {
    Id: int64
    User: User
}

type Persistence() =
    let dc = ProviderDefinition.Sql.GetDataContext()
    
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
         let recordToUser (record: ProviderDefinition.Sql.dataContext.``main.UsersEntity``) =
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

