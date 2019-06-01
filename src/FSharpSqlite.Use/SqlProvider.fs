module FsharpSqlite.Use.SqlProvider

open FSharp.Data.Sql

[<Literal>]
let resolutionPath = __SOURCE_DIRECTORY__ + "/ProviderPackages"

[<Literal>]
let providerConString = "Data Source=" + __SOURCE_DIRECTORY__ + "/provider.db;"

type Sql = SqlDataProvider<Common.DatabaseProviderTypes.SQLITE,
                           providerConString,
                           ResolutionPath = resolutionPath,
                           SQLiteLibrary=Common.SQLiteLibrary.MicrosoftDataSqlite,
                           CaseSensitivityChange = Common.CaseSensitivityChange.ORIGINAL>

