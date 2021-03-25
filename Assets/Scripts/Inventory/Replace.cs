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
                var assetPath = "Assets/Prefabs/Inventory/" + catList[index] + ".asset";
                slot.item = (Pokemon)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Pokemon));
            }
            else
            {
                slot.item = (Pokemon)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Inventory/No Cat.asset", typeof(Pokemon)); ;
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
        curCatSlot.GetComponent<ReplaceSlots>().item = (Pokemon)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Inventory/" + catName + ".asset", typeof(Pokemon));
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
