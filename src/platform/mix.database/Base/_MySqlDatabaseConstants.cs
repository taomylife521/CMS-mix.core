﻿namespace Mix.Database.Base
{
    public class MySqlDatabaseConstants : IDatabaseConstants
    {

        string IDatabaseConstants.DatabaseCollation => "utf8_unicode_ci";

        string IDatabaseConstants.CharSet => "utf8";

        string IDatabaseConstants.SmallLength => "(50)";

        string IDatabaseConstants.MediumLength => "(250)";

        string IDatabaseConstants.MaxLength => "(2000)";

        string IDatabaseConstants.DateTime => "datetime";

        string IDatabaseConstants.Date => "datetime";

        string IDatabaseConstants.Guid => "varchar(255)";

        string IDatabaseConstants.Integer => "int";

        string IDatabaseConstants.Long => "BigInt";

        string IDatabaseConstants.String => "varchar";

        string IDatabaseConstants.NString => "varchar";

        string IDatabaseConstants.Text => "longtext";

        string IDatabaseConstants.GenerateUUID => "(uuid())";

        string IDatabaseConstants.Now => "CURRENT_TIMESTAMP";

        string IDatabaseConstants.Boolean => "tinyint";

        string IDatabaseConstants.BacktickOpen => "`";

        string IDatabaseConstants.BacktickClose => "`";

        string IDatabaseConstants.Time => "time";
    }
}
