module FsharpSqlite.Use.SqlProvider


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
                              
type Sql = ProviderDefinition.Sql
