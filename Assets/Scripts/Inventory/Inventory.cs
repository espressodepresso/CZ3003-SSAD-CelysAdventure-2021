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
                var assetPath = "Prefabs/Inventory/" + catList[index];
                slot.item = Resources.Load<Pokemon>(assetPath);
            }
            else
            {
                slot.item = (Pokemon)Resources.Load("Prefabs/Inventory/No Cat"); ;
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
            CloseInventory();
        }
    }

    public void CloseInventory()
    {
        Debug.Log("click");
        OnCloseInventory();
    }
}
