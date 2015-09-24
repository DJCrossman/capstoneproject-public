using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

public class MatchResultDataServices
{
    private readonly MySqlConnectionServices _mySqlConnectionManager = new MySqlConnectionServices();
    private MySqlConnection _mySqlConnection;

    public void CreateMatchResult(MatchResultData data)
    {
        // TODO: Refactor string MySql queries
        var query = "INSERT INTO MatchResult (MID, UID, Kills, Deaths, Wins, Losses)" +
                    "VALUES('" + data.MID + "', '" + data.UID + "'," +
                        data.Kills + "," + data.Deaths + "," +
                        data.Wins + "," + data.Losses +
                        "', Date('" + data.DateOfMatch.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'))";

        using (_mySqlConnection = new MySqlConnection(_mySqlConnectionManager.ConnectionString()))
        {
            using (var command = new MySqlCommand(query, _mySqlConnection))
            {
                try
                {
                    _mySqlConnection.Open();
                    command.ExecuteNonQuery();
                    _mySqlConnection.Close();
                }
                catch (SqlException e)
                {
                    throw new Exception(e.ToString());
                }

            }
        }
    }

    public MatchResultData GetMatchResultByID(Guid mid)
    {
        var data = new MatchResultData();
        // TODO: Refactor string MySql queries
        var query = "SELECT * FROM MatchResult WHERE MID = '" + mid + "' LIMIT 1";

        using (_mySqlConnection = new MySqlConnection(_mySqlConnectionManager.ConnectionString()))
        {
            using (var command = new MySqlCommand(query, _mySqlConnection))
            {
                try
                {
                    _mySqlConnection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data = new MatchResultData()
                            {
                                DateOfMatch = DateTime.Parse(reader.GetString(reader.GetOrdinal("DateOfMatch"))),
                                MID = new Guid(reader.GetString(reader.GetOrdinal("UID"))),
                                UID = new Guid(reader.GetString(reader.GetOrdinal("UID"))),
                                Kills = reader.GetInt32(reader.GetOrdinal("Kills")),
                                Deaths = reader.GetInt32(reader.GetOrdinal("Deaths")),
                                Wins = reader.GetInt32(reader.GetOrdinal("Wins")),
                                Losses = reader.GetInt32(reader.GetOrdinal("Losses"))
                            };
                        }
                    }
                    _mySqlConnection.Close();
                }
                catch (SqlException e)
                {
                    throw new Exception(e.ToString());
                }

            }
        }

        return data;
    }

    public List<MatchResultData> GetMatchResultsByUserLatestMatches(Guid uid, int count)
    {
        var data = new List<MatchResultData>();
        // TODO: Refactor string MySql queries
        var query = "SELECT * FROM MatchResult WHERE UID = '" + uid + "' ORDER BY DateOfMatch DESC LIMIT " + count;

        using (_mySqlConnection = new MySqlConnection(_mySqlConnectionManager.ConnectionString()))
        {
            using (var command = new MySqlCommand(query, _mySqlConnection))
            {
                try
                {
                    _mySqlConnection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data.Add(new MatchResultData()
                            {
                                DateOfMatch = DateTime.Parse(reader.GetString(reader.GetOrdinal("DateOfMatch"))),
                                MID = new Guid(reader.GetString(reader.GetOrdinal("UID"))),
                                UID = new Guid(reader.GetString(reader.GetOrdinal("UID"))),
                                Kills = reader.GetInt32(reader.GetOrdinal("Kills")),
                                Deaths = reader.GetInt32(reader.GetOrdinal("Deaths")),
                                Wins = reader.GetInt32(reader.GetOrdinal("Wins")),
                                Losses = reader.GetInt32(reader.GetOrdinal("Losses"))
                            });
                        }
                    }
                    _mySqlConnection.Close();
                }
                catch (SqlException e)
                {
                    throw new Exception(e.ToString());
                }

            }
        }

        return data;
    }

    // TODO: Review Function Necessity, should not be able to modify results
    public void UpdateMatchResult(MatchResultData data)
    {
        // TODO: Refactor string MySql queries
        var query = "UPDATE MatchResult" +
                    "SET UID='" + data.UID + "', " +
                    "Kills=" + data.Kills + ", Deaths=" + data.Deaths + ", " +
                    "Wins=" + data.Wins + ", Losses=" + data.Losses +
                    "WHERE MID='" + data.MID + "'";

        using (_mySqlConnection = new MySqlConnection(_mySqlConnectionManager.ConnectionString()))
        {
            using (var command = new MySqlCommand(query, _mySqlConnection))
            {
                try
                {
                    _mySqlConnection.Open();
                    command.ExecuteNonQuery();
                    _mySqlConnection.Close();
                }
                catch (SqlException e)
                {
                    throw new Exception(e.ToString());
                }

            }
        }
    }

    public void DeleteMatchResultByID(Guid mid)
    {
        // TODO: Refactor string MySql queries
        var query = "DELETE * FROM MatchResult WHERE MID = '" + mid + "'";

        using (_mySqlConnection = new MySqlConnection(_mySqlConnectionManager.ConnectionString()))
        {
            using (var command = new MySqlCommand(query, _mySqlConnection))
            {
                try
                {
                    _mySqlConnection.Open();
                    command.ExecuteNonQuery();
                    _mySqlConnection.Close();
                }
                catch (SqlException e)
                {
                    throw new Exception(e.ToString());
                }

            }
        }
    }
}