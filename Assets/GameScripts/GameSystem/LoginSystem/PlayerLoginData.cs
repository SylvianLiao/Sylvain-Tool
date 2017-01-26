using System;

[Serializable]
public class PlayerLoginData
{
    public int UserId;
    public string Password;
    public string Token;

    public PlayerLoginData()
    {
        UserId = -1;
        Password = "12345";
        Token = "";
    }
}
