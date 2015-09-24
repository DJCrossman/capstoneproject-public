using System;
using MySql.Data.MySqlClient;

public class MySqlConnectionServices
{
    private readonly MySqlConnectionStringBuilder _builder;

    public MySqlConnectionServices()
    {
        try {
            _builder = new MySqlConnectionStringBuilder
            {
                Server = "54.200.166.90",
                Port = 3306,
                Database = "capstoneproject",
                UserID = "root",
                Password = "LolCat5!"
            };
        } catch (Exception e) {
            throw new Exception(e.ToString());
        }
    }

    public string ConnectionString()
    {
        return _builder.ConnectionString;
    }
}
