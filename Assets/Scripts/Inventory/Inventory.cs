using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Inventory : MonoBehaviour
{
    public List<Item> list = new List<Item>();
    public GameObject InventoryPanel;
    public GameObject playerDataManager;
    public static Inventory instance;
    public event Action OnCloseInventory;


    public void updatePanelSlots()
    {
        playerDataManager = GameObject.FindGameObjectWithTag("DataManager");

        var catList = playerDataManager.GetComponent<DataManager>().getCurCats();
        int index = 0;
        foreach (Transform child in InventoryPanel.transform)
        {
            inventorySlotController slot = child.GetComponent<inventorySlotController>();
            if (index < catList.Count)
            {
                var assetPath = "Assets/Prefabs/Inventory/" + catList[index] + ".asset";
                slot.item = (Pokemon)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Pokemon));
            }
            else
            {
                slot.item = (Pokemon)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Inventory/No Cat.asset", typeof(Pokemon)); ;
            }
            slot.updateInfo();
            index++;
        }
    }

    public void Add(Item item)
    {
        if (list.Count < 6)
        {
            list.Add(item);
        }
        updatePanelSlots();
    }

    public void Remove(Item item)
    {
        list.Remove(item);
        updatePanelSlots();
    }

    public void Start()
    {
        instance = this;
        updatePanelSlots();
    }

    public void HandleUpdate()
    {
        updatePanelSlots();
        if (Input.GetKeyDown(KeyCode.I))
        {
            OnCloseInventory();
        }
    }
}
