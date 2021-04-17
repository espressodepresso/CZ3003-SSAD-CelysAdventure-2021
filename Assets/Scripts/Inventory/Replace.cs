using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Replace : MonoBehaviour
{
    public List<Item> list = new List<Item>();
    public GameObject ReplacePanel;
    public GameObject playerDataManager;
    public static Replace instance;
    public event Action OnCloseInventory;
    public event Action OnReplaceComplete;


    public void updatePanelSlots()
    {
        playerDataManager = GameObject.FindGameObjectWithTag("DataManager");

        var catList = playerDataManager.GetComponent<DataManager>().getCurCats();
        int index = 0;
        foreach (Transform child in ReplacePanel.transform)
        {
            ReplaceSlots slot = child.GetComponent<ReplaceSlots>();

            if (index < catList.Count)
            {
                var assetPath = "Prefabs/Inventory/" + catList[index];
                slot.item = Resources.Load<Pokemon>(assetPath);
            }
            else
            {
                slot.item = (Pokemon)Resources.Load("Prefabs/Inventory/No Cat"); ;
            }
            slot.updateSlot();
            index++;
        }
    }
    public void updateCurrentCat()
    {
        playerDataManager = GameObject.FindGameObjectWithTag("DataManager");
        var catName = playerDataManager.GetComponent<DataManager>().getQuizCat();
        Debug.Log(catName);
        var curCatSlot = GameObject.FindGameObjectWithTag("CurrentCat");
        curCatSlot.GetComponent<ReplaceSlots>().item = Resources.Load<Pokemon>("Prefabs/Inventory/" + catName);
        curCatSlot.GetComponent<ReplaceSlots>().updateSlot();
    }

    public void Add(Item item)
    {
        if (list.Count < 6)
        {
            list.Add(item);
        }
        updatePanelSlots();
    }

    public void ReplaceCat()
    {
        var catList = playerDataManager.GetComponent<DataManager>().getCurCats();
        var curCatSlot = GameObject.FindGameObjectWithTag("CurrentCat");
        bool discardFlag = curCatSlot.GetComponent<ReplaceSlots>().exitReplace;
        if (discardFlag) 
        { 
            curCatSlot.GetComponent<ReplaceSlots>().exitReplace = false;
            OnReplaceComplete(); 
        }
        int index = 0;
        foreach (Transform child in ReplacePanel.transform)
        {
            ReplaceSlots slot = child.GetComponent<ReplaceSlots>();
            if (slot.toBeReplaced == true)
            {
                slot.toBeReplaced = false;
                OnReplaceComplete();
            }
            index++;
        }


    }

    public void Start()
    {
        instance = this;
        updatePanelSlots();
    }

    public void HandleUpdate()
    {
        updateCurrentCat();
        updatePanelSlots();
        ReplaceCat();
        if (Input.GetKeyDown(KeyCode.I))
        {
            OnCloseInventory();
        }
    }
}
