using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplaceSlots : MonoBehaviour
{
    public Item item;
    public bool toBeReplaced;
    public bool exitReplace;
    public GameObject playerDataManager;
    public void updateSlot()
    {
        Text displayText = transform.Find("Text").GetComponent<Text>();
        Image displayImage = transform.Find("Image").GetComponent<Image>();

        if (item)
        {
            displayText.text = item.itemName;
            displayImage.sprite = item.icon;
            displayImage.color = Color.white;

        }

        else
        {
            displayText.text = "";
            displayImage.sprite = null;
            displayImage.color = Color.clear;

        }
    }
    public void Replace()
    {
        toBeReplaced = true;
        playerDataManager = GameObject.FindGameObjectWithTag("DataManager");
        var newCat = playerDataManager.GetComponent<DataManager>().getQuizCat();
        playerDataManager.GetComponent<DataManager>().replaceCat(item.itemName,newCat);
    }
    public void exit()
    {
        exitReplace=true;
    }
    public void HandleUpdate()
    {
        updateSlot();
    }
}
