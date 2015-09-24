using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IUserDataServices
{
    void CreateUser(UserData data);
    UserData GetUserByLogin(string email);
    void UpdateUserName(UserData data);
    void DeleteUserByID(Guid uid);
    void CreateLocalProfile(UserData userData);
    UserData LoadLocalProfile();
    void UpdateLocalProfile(UserData userData);
    void DeleteLocalProfile();
}
