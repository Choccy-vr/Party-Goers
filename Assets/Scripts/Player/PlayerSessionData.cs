using System;
[Serializable]
public class PlayerSessionData
{
    public ulong networkClientId;
    public int coins;
    public int stars;
    public int lastBoardSpaceId;
    public string username;

    public PlayerSessionData(ulong clientId, string name)
    {
        networkClientId = clientId;
        coins = 0;
        stars = 0;
        lastBoardSpaceId = 0;
        username = name;
    }
}
