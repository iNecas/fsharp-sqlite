module FsharpSqlite.AddUsers

open FluentMigrator

[<Migration(20190523212845L)>]
type AddUsers() =
    inherit Migration()

    override m.Up () =
        m.Create.Table("Users")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("FirstName").AsString()
            .WithColumn("LastName").AsString()
            .WithColumn("Email").AsString()
            .WithColumn("PasswordHash").AsString()
            .WithColumn("PasswordSalt").AsString()
        |> ignore
        
    override m.Down () =
        m.Delete.Table("Users")
        |> ignore
