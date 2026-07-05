using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    public List<ItemConfig> items = new List<ItemConfig>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        Instance = null;
    }

    public ItemConfig FindItemByID(String ID)
    {
        return items.Find(i => i.itemID == ID);
    }

    public void UseItem(ItemConfig item)
    {
        item.ActivateOnItemUsedEvent();
    }

    public void AddItemToInventory(ItemConfig item)
    {
        GameSessionManager.Instance.AddItemToPlayer(NetworkManager.Singleton.LocalClientId, item.itemID);
    }
}
