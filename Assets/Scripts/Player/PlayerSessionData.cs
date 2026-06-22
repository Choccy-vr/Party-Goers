using System;
using Unity.Netcode;
using Unity.Collections;

[Serializable]
public struct PlayerSessionData : INetworkSerializable, IEquatable<PlayerSessionData>
{
    public ulong networkClientId;
    public int coins;
    public int stars;
    public int lastBoardSpaceId;
    public FixedString32Bytes username; // Max 32 characters

    public PlayerSessionData(ulong clientId, string name)
    {
        networkClientId = clientId;
        coins = 0;
        stars = 0;
        lastBoardSpaceId = 0;
        username = name;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref networkClientId);
        serializer.SerializeValue(ref coins);
        serializer.SerializeValue(ref stars);
        serializer.SerializeValue(ref lastBoardSpaceId);
        serializer.SerializeValue(ref username);
    }

    public bool Equals(PlayerSessionData other)
    {
        return networkClientId == other.networkClientId;
    }
}