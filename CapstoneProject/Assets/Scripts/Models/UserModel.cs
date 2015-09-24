using System;

[Serializable]
public class UserModel
{
	public string UserName;
	public Guid Uid;
	public bool Dead;
    public int ColorIndex;
    public int CurrentTeam;
    public int NextTeam;
    public int Kills;
    public int Deaths;
    public int Money;
    public int Ping;
    public int Points;
    public int NetworkIdHash;
    public int Weapon;
}
