using System;

[Serializable]
public class UserData {
    public Guid UID { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime DateCreated { get; set; }
	public UserData () {
		this.UID = Guid.NewGuid();
		this.UserName = "u" + this.UID.ToString().Substring(0,7);
	    this.Email = "";
	    this.Password = "";
		this.DateCreated = DateTime.Now;
	}

}