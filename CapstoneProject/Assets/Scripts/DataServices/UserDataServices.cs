using System;
using System.IO;
using System.Collections;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using MySql.Data.MySqlClient;

public class UserDataServiceses : IUserDataServices
{
    private readonly MySqlConnectionServices _mySqlConnectionService = new MySqlConnectionServices();
    private MySqlConnection _mySqlConnection;
    public string LocalFilePath { get; set; }

    /**************************************************
     * Server Data Services
     **************************************************/

    public void CreateUser(UserData data)
    {
        // TODO: Refactor string MySql queries
        var query = "INSERT INTO User (UID, Username, Email, Password, DateCreated)" +
                    "VALUES('" + data.UID + "', '" + data.UserName + "','" +
                        data.Email + "','" + data.Password + "', Date('" + data.DateCreated.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'))";

        using (_mySqlConnection = new MySqlConnection(_mySqlConnectionService.ConnectionString()))
        {
            using (var command = new MySqlCommand(query, _mySqlConnection))
            {
                try
                {
                    _mySqlConnection.Open();
                    command.ExecuteNonQuery();
                    _mySqlConnection.Close();
                    Console.print("Created User Successfully");
                }
                catch (SqlException e)
                {
                    throw new Exception(e.ToString());
                }

            }
        }
    }

    public UserData GetUserByLogin(string email)
    {
        UserData user = null;
        // TODO: Refactor string MySql queries
        var query = "SELECT * FROM User WHERE Email = '" + email + "' LIMIT 1";

        using (_mySqlConnection = new MySqlConnection(_mySqlConnectionService.ConnectionString()))
        {
            using (var command = new MySqlCommand(query, _mySqlConnection))
            {
                try
                {
                    _mySqlConnection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read()) {
                            user = new UserData()
                            {
                                DateCreated = DateTime.Parse(reader.GetString(reader.GetOrdinal("DateCreated"))),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                UID = new Guid(reader.GetString(reader.GetOrdinal("UID"))),
                                UserName = reader.GetString(reader.GetOrdinal("UserName"))
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

        if(user == null)
            throw new Exception("Login to Capstone Project Failed." + Environment.NewLine +
                "Either the specified account does not exist, or the password was wrong." + Environment.NewLine +
                "Please check your account email and password and try again.");

        return user;
    }

    public void UpdateUserName(UserData data)
    {
        // TODO: Refactor string MySql queries
        var query = "UPDATE User " +
                    "SET Username='" + data.UserName + "'" +
                    "WHERE UID='" + data.UID + "'";

        using (_mySqlConnection = new MySqlConnection(_mySqlConnectionService.ConnectionString()))
        {
            using (var command = new MySqlCommand(query, _mySqlConnection))
            {
                try
                {
                    _mySqlConnection.Open();
                    command.ExecuteNonQuery();
                    _mySqlConnection.Close();
                    Console.print("Username updated Successfully");
                }
                catch (SqlException e)
                {
                    throw new Exception(e.ToString());
                }

            }
        }
    }

    public void DeleteUserByID(Guid uid)
    {
        // TODO: Refactor string MySql queries
        var query = "DELETE * FROM User WHERE UID = '" + uid + "'";

        using (_mySqlConnection = new MySqlConnection(_mySqlConnectionService.ConnectionString()))
        {
            using (var command = new MySqlCommand(query, _mySqlConnection))
            {
                try
                {
                    _mySqlConnection.Open();
                    command.ExecuteNonQuery();
                    _mySqlConnection.Close();
                    Console.print("User Deleted Successfully");
                }
                catch (SqlException e)
                {
                    throw new Exception(e.ToString());
                }

            }
        }
    }

    /**************************************************
     * Local Data Services
     **************************************************/
    // TODO: Review Security of local storage

    public void CreateLocalProfile(UserData userData)
    {
        if (userData == null) throw new ArgumentNullException("userData");
        var bf = new BinaryFormatter();
        var file = File.Create(LocalFilePath);

        bf.Serialize(file, userData);
        file.Close();
    }

    public UserData LoadLocalProfile()
    {
        var bf = new BinaryFormatter();
        var file = File.Open(LocalFilePath, FileMode.Open);
        var data = (UserData)bf.Deserialize(file);
        file.Close();
        return data;
    }

    public void UpdateLocalProfile(UserData userData)
    {
        var bf = new BinaryFormatter();
        var file = File.Open(LocalFilePath, FileMode.Open);

        bf.Serialize(file, userData);
        file.Close();
    }

    public void DeleteLocalProfile()
    {
        if (File.Exists(LocalFilePath))
            File.Delete(LocalFilePath);
    }

    
}
