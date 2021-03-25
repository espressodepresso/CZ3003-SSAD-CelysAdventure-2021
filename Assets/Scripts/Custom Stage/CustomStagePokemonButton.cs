using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomStagePokemonButton : MonoBehaviour
{
    public GameObject customStageUIManager;
    public GameObject selectedGlow;
    private bool selectedState;
    private GameObject[] backgrounds;
    // Update is called once per frame
    void Update()
    {
        
    }
    public void choosePokemon()
    {
        customStageUIManager.GetComponent<CustomStageUIManager>().SelectPokemon(this.name);
        selectedState = !selectedState;
        selectedGlow.SetActive(selectedState);
       
        
    }
    public void chooseBackground()
    {
        customStageUIManager.GetComponent<CustomStageUIManager>().SelectBackground(this.name);

        selectedState = !selectedState;
        backgrounds = GameObject.FindGameObjectsWithTag("background");
        foreach(GameObject background in backgrounds)
        {
            Outline o = background.GetComponent<Outline>();
            o.enabled = false;
        }
        this.GetComponent<Outline>().enabled = true;
    }
}
