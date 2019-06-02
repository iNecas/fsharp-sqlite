module FSharpSqlite.Use.Domain

type Convert = System.Convert

module internal Text =
    type Encoding = System.Text.Encoding
    
module internal Crypto =
    open System.Security
    type RNGCryptoServiceProvider = Cryptography.RNGCryptoServiceProvider
    type SHA256 = Cryptography.SHA256
    
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


