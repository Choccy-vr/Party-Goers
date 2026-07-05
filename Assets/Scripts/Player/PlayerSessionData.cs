using System;
using Unity.Netcode;
using Unity.Collections;
using System.Collections.Generic;

[Serializable]
public struct PlayerSessionData : INetworkSerializable, IEquatable<PlayerSessionData>
{
    public ulong networkClientId;
    public int coins;
    public int stars;
    public int lastBoardSpaceId;
    public FixedString32Bytes username; // Max 32 characters

    public const int MAX_INVENTORY_SIZE = 3;
    public int currentInventoryCount;

    private FixedString32Bytes item1;
    private FixedString32Bytes item2;
    private FixedString32Bytes item3;

    public PlayerSessionData(ulong clientId, string name)
    {
        networkClientId = clientId;
        coins = 0;
        stars = 0;
        lastBoardSpaceId = 0;
        username = name;
        currentInventoryCount = 0;
        item1 = default;
        item2 = default;
        item3 = default;
    }

    // Helper methods to interact with your items cleanly
    public string GetItemAt(int index)
    {
        if (index < 0 || index >= currentInventoryCount) return string.Empty;
        return index switch
        {
            0 => item1.ToString(),
            1 => item2.ToString(),
            2 => item3.ToString(),
            _ => string.Empty
        };
    }

    public bool TryAddItem(string itemName)
    {
        if (currentInventoryCount >= MAX_INVENTORY_SIZE) return false;

        FixedString32Bytes fixedName = itemName;
        switch (currentInventoryCount)
        {
            case 0: item1 = fixedName; break;
            case 1: item2 = fixedName; break;
            case 2: item3 = fixedName; break;
        }
        currentInventoryCount++;
        return true;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref networkClientId);
        serializer.SerializeValue(ref coins);
        serializer.SerializeValue(ref stars);
        serializer.SerializeValue(ref lastBoardSpaceId);
        serializer.SerializeValue(ref username);

        //Inventory

        serializer.SerializeValue(ref currentInventoryCount);
        serializer.SerializeValue(ref item1);
        serializer.SerializeValue(ref item2);
        serializer.SerializeValue(ref item3);
    }

    public bool Equals(PlayerSessionData other)
    {
        // Netcode uses this to see if the NetworkVariable changed.
        // Include inventory tracking if items changing should trigger a network sync!
        return networkClientId == other.networkClientId &&
               coins == other.coins &&
               stars == other.stars &&
               lastBoardSpaceId == other.lastBoardSpaceId &&
               username == other.username &&
               currentInventoryCount == other.currentInventoryCount &&
               item1 == other.item1 &&
               item2 == other.item2 &&
               item3 == other.item3;
    }
}