using System;
using System.IO;

public class UserServices
{
    public IUserDataServices UserDataServices;
    private UserData _userData;
    public int SaltBytes = 24;
    public char Delimiter = ':';

	public UserServices (string filePath)
	{
        UserDataServices = new UserDataServiceses() {
            LocalFilePath = filePath
        };
		if(File.Exists(filePath))
		{
		    var data = UserDataServices.LoadLocalProfile();
		    var tempUser = UserDataServices.GetUserByLogin(data.Email);
		    if (ValidPassword(data.Password, tempUser.Password))
		        _userData = tempUser;
		}
	}

    public void VerifyLoginInformation(string email, string password, bool remMyPassword)
    {
        var tempUser = UserDataServices.GetUserByLogin(email);
        if (ValidPassword(password, tempUser.Password)) {
            _userData = tempUser;
            if (remMyPassword) {
                UserDataServices.CreateLocalProfile(_userData);
            }
        }
    }

    public bool ValidPassword(string a, string b)
    {

        string[] strA = a.Split(Delimiter);
        string[] strB = b.Split(Delimiter);

        return strA[1].Equals(strB[1]);
    }

    public void CreateAccount(string userName, string email, string password)
    {
        if(userName == "" || email == "" || password == "")
            throw new Exception("Username, Email, and Password cannot be empty.");
        var data = new UserData() {
            UserName = userName,
            Email = email,
            Password = password
        };
        UserDataServices.CreateUser(data);
        _userData = data;
    }

    public void LogOut()
    {
        _userData = null;
        UserDataServices.DeleteLocalProfile();
    }

    /**************************************************
	 * Getters and Setters
	 **************************************************/

	public string GetUserName()
	{
		return _userData.UserName;
	}

    public void SetUserName(string name)
    {
        if (_userData == null)
            throw new Exception("That's pretty neat. There is no user set.");
        if (name == "")
            throw new Exception("Username cannot be empty.");
        _userData.UserName = name;
        UserDataServices.UpdateLocalProfile(_userData);
        UserDataServices.UpdateUserName(_userData);
    }

    public UserData GetUserData()
	{
		return _userData;
	}
}
