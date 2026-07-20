using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopProvider : MonoBehaviour
{
    [SerializeField] List<ItemConfig> items = new List<ItemConfig>();

    [SerializeField] Transform[] spawnPositions;

    List<GameObject> spawnedItems = new List<GameObject>();
    void Start()
    {
        MapManager.Instance.shops.Add(this);
    }

    public void spawnItems()
    {
        if (items.Count != spawnPositions.Length)
        {
            Debug.LogError("Items length and spaen positions length must be the same!");
            return;
        }
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            Debug.Log("Item" + items[i].name);
            GameObject item = Instantiate(items[i].shopItemPrefab, spawnPositions[i].position, spawnPositions[i].rotation);
            spawnedItems.Add(item);
            item.GetComponent<ShopItem>().itemOnSale = items[i];
        }
    }
    public void despawnItems()
    {
        foreach (GameObject item in spawnedItems)
        {
            Destroy(item);
        }
    }

}
